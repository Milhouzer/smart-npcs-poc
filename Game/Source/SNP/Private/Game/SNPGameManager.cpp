#include "Game/SNPGameManager.h"

#include "Game/SNPGameModeBase.h"
#include "Game/LevelSimulation/LevelSimulationSubsystem.h"
#include "GameFramework/PlayerController.h"
#include "Utils/Utility.h"

class ASNPGameModeBase;

void USNPGameManager::Initialize(ASNPGameModeBase* GameMode)
{
	if (!VALIDATE_PARAMS(GameMode)) { return; }
    
    CachedGameMode = GameMode;
    
    // Bind to the GameInstance's dynamic delegates
    GameMode->OnPlayerConnected.AddDynamic(this, &USNPGameManager::HandlePlayerConnected);
    GameMode->OnPlayerDisconnected.AddDynamic(this, &USNPGameManager::HandlePlayerDisconnected);

    UE_LOG(LogTemp, Log, TEXT("SNPGameManager: Successfully initialized."));
}

void USNPGameManager::Deinitialize(ASNPGameModeBase* GameMode)
{
	if (!VALIDATE_PARAMS(GameMode)) { return; }
    
    // Unbind from the GameInstance's dynamic delegates
    GameMode->OnPlayerConnected.RemoveDynamic(this, &USNPGameManager::HandlePlayerConnected);
    GameMode->OnPlayerDisconnected.RemoveDynamic(this, &USNPGameManager::HandlePlayerDisconnected);

    CachedGameMode.Reset();

    UE_LOG(LogTemp, Log, TEXT("SNPGameManager: Successfully deinitialized."));
}

void USNPGameManager::HandlePlayerConnected(APlayerController* PlayerController)
{
	if (!VALIDATE_PARAMS(PlayerController)) { return; }
    
    if (IsServer())
    {
        HandlePlayerConnected_Server(PlayerController);
    }
    else
    {
        HandlePlayerConnected_Client(PlayerController);
    }
}

void USNPGameManager::HandlePlayerDisconnected(APlayerController* PlayerController)
{
	if (!VALIDATE_PARAMS(PlayerController)) { return; }
    
    UE_LOG(LogTemp, Log, TEXT("Player Disconnected: %s"), *PlayerController->GetName());
    if (IsServer())
    {
        HandlePlayerDisconnected_Server(PlayerController);
    }
    else
    {
        HandlePlayerDisconnected_Client(PlayerController);
    }
}

void USNPGameManager::HandlePlayerConnected_Server(APlayerController* PlayerController) const
{
    UE_LOG(LogTemp, Log, TEXT("Player connected on the server: %s"), *PlayerController->GetName());
    // SimulatePlayerInteriorLevel(PlayerController);
}

void USNPGameManager::HandlePlayerConnected_Client(APlayerController* PlayerController) const
{
    UE_LOG(LogTemp, Log, TEXT("Player connected on the client: %s"), *PlayerController->GetName());
}

void USNPGameManager::HandlePlayerDisconnected_Server(APlayerController* PlayerController) const
{
    UE_LOG(LogTemp, Log, TEXT("Player disconnected on the server: %s"), *PlayerController->GetName());
}

void USNPGameManager::HandlePlayerDisconnected_Client(APlayerController* PlayerController) const
{
    UE_LOG(LogTemp, Log, TEXT("Player disconnected on the client: %s"), *PlayerController->GetName());
}

// Called every frame
void USNPGameManager::Tick(float DeltaTime)
{
    // Perform your tick logic here
    // if (IsServer())
    // {
    //     LvlManager->Tick(DeltaTime);
    // }
}

bool USNPGameManager::IsTickable() const
{
    // Only tick if the manager is active
    return CachedGameMode.IsValid();
}

TStatId USNPGameManager::GetStatId() const
{
    // Return a unique stat ID for performance profiling
    RETURN_QUICK_DECLARE_CYCLE_STAT(USNPGameManager, STATGROUP_Tickables);
}


bool USNPGameManager::IsServer() const
{
    if (CachedGameMode.IsValid())
    {
        UWorld* World = CachedGameMode->GetWorld();
        return World && (World->IsNetMode(NM_Standalone) || World->IsNetMode(ENetMode::NM_DedicatedServer) || World->IsNetMode(ENetMode::NM_ListenServer));
    }
    return false;
}

