#include "Game/LevelSimulation/Components/SimulationRenderer.h"

#include "Game/LevelSimulation/LevelSimulationSubsystem.h"
#include "Game/LevelSimulation/LevelSimulationData.h"
#include "Utils/Utility.h"
#include "CoreMinimal.h"

void USimulationRenderer::GetLifetimeReplicatedProps(TArray<FLifetimeProperty>& OutLifetimeProps) const
{
    Super::GetLifetimeReplicatedProps(OutLifetimeProps);
}

void USimulationRenderer::BeginPlay()
{
    Super::BeginPlay();
}

void USimulationRenderer::Init()
{
    if (!GetWorld())
    {
        UE_LOG(LogTemp, Warning, TEXT("Invalid World"));
        return;
    }
    
	if(!VALIDATE_PARAMS(SimulationDataAsset)) { return; }
    
    FLevelSimulationData* Data = new FLevelSimulationData(SimulationDataAsset);
    const FVector Location = GetRelativeLocation() + Data->ExitTransform.GetLocation();
    Data->ExitTransform.SetLocation(Location);

    const UGameInstance* GameInstance = GetWorld()->GetGameInstance();
    if (ULevelsManager* LevelsManager = GameInstance->GetSubsystem<ULevelsManager>())
    {
        // TODO: This should be done the other way (LevelsManager instantiating saved simulations)
        // ---> NO ?
        Simulation = LevelsManager->AddSimulation(this, Data);
    }
}

void USimulationRenderer::Render(AActor* Actor)
{
    if(!VALIDATE_PARAMS(Actor)) { return; }

    APlayerController* PlayerController = Cast<APlayerController>(Actor->GetOwner());
    if (!PlayerController)
    {
        UE_LOG(LogTemp, Warning, TEXT("Invalid PlayerController"));
        return;
    }
    
    if (!GetWorld())
    {
        UE_LOG(LogTemp, Warning, TEXT("Invalid World"));
        return;
    }
    
    UGameInstance* GameInstance = GetWorld()->GetGameInstance();
    if (ULevelsManager* LevelsManager = GameInstance->GetSubsystem<ULevelsManager>())
    {
        if(LevelsManager->Render(this, PlayerController))
        {
            Simulation->Enter(PlayerController);
        }
    }
}

void USimulationRenderer::StopRender(AActor* Actor)
{
    if(!VALIDATE_PARAMS(Actor)) { return; }

    APlayerController* PlayerController = Cast<APlayerController>(Actor->GetOwner());
    if (!PlayerController)
    {
        UE_LOG(LogTemp, Warning, TEXT("Invalid PlayerController"));
        return;
    }
    
    if (!GetWorld())
    {
        UE_LOG(LogTemp, Warning, TEXT("Invalid World"));
        return;
    }
    
    const UGameInstance* GameInstance = GetWorld()->GetGameInstance();
    if (ULevelsManager* LevelsManager = GameInstance->GetSubsystem<ULevelsManager>())
    {
        if(LevelsManager->StopRender(this, PlayerController))
        {
            Simulation->Exit(PlayerController);
        }
    }
}

