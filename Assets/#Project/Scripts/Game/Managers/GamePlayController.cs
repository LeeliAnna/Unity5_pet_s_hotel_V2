using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contrôleur responsable de la mise à jour du gameplay en temps réel.
/// Orchestre les updates de tous les chiens, du niveau, de la caméra, du HUD, etc.
/// Appelé uniquement pendant l'état PlayingState ou EndOfDayState.
/// Architecture prête pour multi-chiens.
/// </summary>
public class GameplayController
{
    private readonly List<DogBehaviour> dogs;
    private readonly LevelManager levelManager;
    private readonly CameraManager cameraManager;
    private readonly PlayerInteraction playerInteraction;
    private readonly AggregateurSatisfactionPension satisfactionService;
    private readonly HudGlobalSatisfaction hudGlobalSatisfaction;
    private readonly PensionController pensionController;

    public GameplayController(
        List<DogBehaviour> dogs,
        LevelManager levelManager,
        CameraManager cameraManager,
        PlayerInteraction playerInteraction,
        AggregateurSatisfactionPension satisfactionService,
        HudGlobalSatisfaction hudGlobalSatisfaction,
        PensionController pensionController)
    {
        this.dogs = dogs;
        this.levelManager = levelManager;
        this.cameraManager = cameraManager;
        this.playerInteraction = playerInteraction;
        this.satisfactionService = satisfactionService;
        this.hudGlobalSatisfaction = hudGlobalSatisfaction;
        this.pensionController = pensionController;
    }

    /// <summary>
    /// Appelé chaque frame par le GameManager pendant le gameplay actif.
    /// </summary>
    public void Process()
    {
        UpdateDogs();
        UpdatePlayerInteraction();
        UpdateCamera();
        UpdateSatisfaction();
        UpdateHUD();
    }

    /// <summary>
    /// Met à jour le comportement de tous les chiens.
    /// </summary>
    private void UpdateDogs()
    {
        if (dogs == null || dogs.Count == 0) return;

        foreach (var dog in dogs)
        {
            if (dog != null)
            {
                dog.Process();
            }
        }
    }

    /// <summary>
    /// Met à jour les interactions du joueur.
    /// </summary>
    private void UpdatePlayerInteraction()
    {
        if (playerInteraction != null)
        {
            playerInteraction.process();
        }
    }

    /// <summary>
    /// Met à jour la caméra.
    /// </summary>
    private void UpdateCamera()
    {
        if (cameraManager != null)
        {
            cameraManager.Process();
        }
    }

    /// <summary>
    /// Recalcule la satisfaction globale.
    /// </summary>
    private void UpdateSatisfaction()
    {
        if (satisfactionService != null)
        {
            satisfactionService.Recompute();
        }
    }

    /// <summary>
    /// Met à jour l'affichage du HUD.
    /// </summary>
    private void UpdateHUD()
    {
        if (hudGlobalSatisfaction == null) return;

        // Mise à jour des données de pension
        Pension pension = pensionController?.CurrentPension;
        if (pension != null)
        {
            hudGlobalSatisfaction.SetPension(pension.Name, pension.Money, pension.Prestige);
        }
        else
        {
            hudGlobalSatisfaction.SetPension("-", 0, 0);
        }

        // Mise à jour de la satisfaction
        hudGlobalSatisfaction.Process();
    }
}