using System;
using System.Collections.Generic;
using System.Threading;
using ControllThemAll.Runtime.Utils;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ControllThemAll.Runtime.Gameplay
{
    public class PlayersMovementController : IDisposable
    {
        private readonly LevelPlayersService levelPlayersService;
        private readonly IHorizontalMovementInput horizontalMovementInput;
        private readonly IAttackInput attackInput;

        private CancellationTokenSource cts;


        public PlayersMovementController(
            LevelPlayersService levelPlayersService,
            IHorizontalMovementInput horizontalMovementInput,
            IAttackInput attackInput)
        {
            this.levelPlayersService = levelPlayersService;
            this.horizontalMovementInput = horizontalMovementInput;
            this.attackInput = attackInput;
        }

        public void SetActive(bool active)
        {
            if (active)
            {
                cts = new CancellationTokenSource();

                ControlPlayersMovement(cts.Token).Forget();
            }
            else
            {
                cts?.Cancel();
                cts?.Dispose();
            }
        }

        public void Dispose()
        {
            cts?.Cancel();
            cts?.Dispose();
        }


        private async UniTask ControlPlayersMovement(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (horizontalMovementInput.Input != 0 && !attackInput.IsActive)
                {
                    List<int> closePlayersIndexes = GetClosePlayersIndexes();

                    SwapClosePlayers(closePlayersIndexes);
                    PushFarPlayers(closePlayersIndexes);

                    while (horizontalMovementInput.Input != 0)
                    {
                        await UniTask.Yield(cancellationToken);
                    }
                }
                else if (attackInput.IsActive)
                {
                    while (attackInput.IsActive)
                    {
                        await UniTask.Yield(cancellationToken);
                    }
                }

                await UniTask.Yield(cancellationToken);
            }
        }

        private List<int> GetClosePlayersIndexes()
        {
            List<int> closePlayersIndexes = new List<int>();

            for (int i = 0; i < levelPlayersService.Players.Count; i++)
            {
                for (int j = i + 1; j < levelPlayersService.Players.Count; j++)
                {
                    if (!ArePlayersMovingTowardsEachOther(levelPlayersService.Players[i], levelPlayersService.Players[j]))
                    {
                        continue;
                    }

                    if (Vector3.Distance(levelPlayersService.Players[i].transform.position, levelPlayersService.Players[j].transform.position)
                        <= RuntimeConstants.Settings.PlayersPositionSwapDistance * 2)
                    {
                        closePlayersIndexes.Add(i);
                        closePlayersIndexes.Add(j);
                    }
                }
            }

            return closePlayersIndexes;
        }

        private void SwapClosePlayers(List<int> closePlayersIndexes)
        {
            for (int i = 0; i < closePlayersIndexes.Count; i += 2)
            {
                float distanceBetweenPlayers = Vector3.Distance(
                    levelPlayersService.Players[i].transform.position,
                    levelPlayersService.Players[i + 1].transform.position);
                float swapDistance = distanceBetweenPlayers > RuntimeConstants.Settings.PlayersPositionSwapDistance
                    ? RuntimeConstants.Settings.PlayersPositionSwapDistance * 2
                    : RuntimeConstants.Settings.PlayersPositionSwapDistance;
                float swapTime = distanceBetweenPlayers > RuntimeConstants.Settings.PlayersPositionSwapDistance
                    ? RuntimeConstants.Settings.PlayersPositionSwapTime * 2
                    : RuntimeConstants.Settings.PlayersPositionSwapTime;
                Vector3 firstPlayerTargetPosition = levelPlayersService.Players[i].transform.position
                        + GetPlayerTargetDirection(levelPlayersService.Players[i]) * swapDistance;
                Vector3 secondPlayerTargetPosition = levelPlayersService.Players[i + 1].transform.position
                        + GetPlayerTargetDirection(levelPlayersService.Players[i + 1]) * swapDistance;

                if (Random.value > 0.5f)
                {
                    levelPlayersService.Players[i].Jump(firstPlayerTargetPosition, swapTime);
                    levelPlayersService.Players[i + 1].Push(secondPlayerTargetPosition, swapTime);
                }
                else
                {
                    levelPlayersService.Players[i].Push(firstPlayerTargetPosition, swapTime);
                    levelPlayersService.Players[i + 1].Jump(secondPlayerTargetPosition, swapTime);
                }
            }
        }

        private void PushFarPlayers(List<int> closePlayersIndexes)
        {
            for (var i = 0; i < levelPlayersService.Players.Count; i++)
            {
                if (closePlayersIndexes.Contains(i))
                {
                    continue;
                }

                Vector3 playerTargetPosition = levelPlayersService.Players[i].transform.position
                        + GetPlayerTargetDirection(levelPlayersService.Players[i]) * RuntimeConstants.Settings.PlayersPositionSwapDistance;

                levelPlayersService.Players[i].Push(playerTargetPosition, RuntimeConstants.Settings.PlayersPositionSwapTime);
            }
        }

        private Vector3 GetPlayerTargetDirection(PlayerView player)
        {
            float input = IsOnSameBrickGameplayLayer(player) ? horizontalMovementInput.Input : -horizontalMovementInput.Input;
            Vector3 pushDirection = new Vector3(input, 0, 1);

            return !IsMovingDirectionTowardsLevelBorders(player, pushDirection)
                ? pushDirection
                : Vector3.forward;
        }

        private bool ArePlayersMovingTowardsEachOther(PlayerView firstPlayer, PlayerView secondPlayer)
        {
            Vector3 firstPlayerTargetDirection = GetPlayerTargetDirection(firstPlayer).normalized;
            Vector3 secondPlayerTargetDirection = GetPlayerTargetDirection(secondPlayer).normalized;
            Vector3 fromFirstToSecondPlayerDirection = (secondPlayer.transform.position - firstPlayer.transform.position).normalized;
            Vector3 fromSecondToFirstPlayerDirection = (firstPlayer.transform.position - secondPlayer.transform.position).normalized;

            float dotFirst = Vector3.Dot(firstPlayerTargetDirection, fromFirstToSecondPlayerDirection);
            float dotSecond = Vector3.Dot(secondPlayerTargetDirection, fromSecondToFirstPlayerDirection);

            return (dotFirst > 0f && dotSecond > 0f) || (dotFirst > 0f && dotSecond == 0) || (dotSecond > 0f && dotFirst == 0);
        }

        private bool IsOnSameBrickGameplayLayer(PlayerView player)
        {
            int brickLayerIndex = LayerMask.NameToLayer(RuntimeConstants.PhysicLayers.Brick);
            LayerMask brickLayerMask = 1 << brickLayerIndex;
            Vector3 raycastPosition = player.transform.position + Vector3.up;

            if (Physics.Raycast(raycastPosition, Vector3.down, out RaycastHit hit, Mathf.Infinity, brickLayerMask))
            {
                if (hit.collider.gameObject.TryGetComponent(out BrickView currentBrick))
                {
                    return currentBrick.GameplayLayer == player.GameplayLayer;
                }
            }

            return false;
        }

        private bool IsMovingDirectionTowardsLevelBorders(PlayerView player, Vector3 direction)
        {
            int environmentPartLayerIndex = LayerMask.NameToLayer(RuntimeConstants.PhysicLayers.EnvironmentPart);
            LayerMask environmentPartLayerMask = 1 << environmentPartLayerIndex;
            Vector3 raycastPosition = player.transform.position;

            if (Physics.Raycast(raycastPosition, direction, RuntimeConstants.Settings.LevelBordersDetectionDistance, environmentPartLayerMask))
            {
                return true;
            }

            return false;
        }
    }
}