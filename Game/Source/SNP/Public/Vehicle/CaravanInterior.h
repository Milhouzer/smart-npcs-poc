// CaravanInterior.h
#pragma once
#include "Game/LevelSimulation/LevelSimulationData.h"
#include "Game/LevelSimulation/Interfaces/SimulatedLevel.h"

#include "CaravanInterior.generated.h"

class ILevelSimulator;

UCLASS(BlueprintType, Blueprintable)
class ACaravanInterior : public AActor, public ISimulatedLevel
{
    GENERATED_BODY()

public:
    ACaravanInterior();

    // Override engine methods
    virtual void BeginPlay() override;
    virtual bool IsNetRelevantFor(const AActor* RealViewer, const AActor* ViewTarget, const FVector& SrcLocation) const override;
    
    // Simulated level interface
    virtual void Init(FLevelSimulationData* SimulationData) override;
    virtual void Simulate(float DeltaTime) override;
    virtual void Enter(APlayerController* PlayerController) override;
    virtual void Exit(APlayerController* PlayerController) override;

    // UFUNCTION(BlueprintCallable)
    // void SetVisiblePlayer(APlayerController* PlayerController, bool Visible);

    // virtual void GetLifetimeReplicatedProps(TArray<FLifetimeProperty>& OutLifetimeProps) const override;

    // UFUNCTION(BlueprintCallable)
    // UStaticMeshComponent* GetMeshComponent() const
    // {
    //     return FloorMeshComponent;
    // }
    
private:
    // Data associated to this simulation
    FLevelSimulationData* Data;
    
    
    // UPROPERTY(VisibleAnywhere)
    // UStaticMeshComponent* FloorMeshComponent;

    // UFUNCTION()
    // void OnRep_VisiblePlayers();
    
    // UPROPERTY(ReplicatedUsing=OnRep_VisiblePlayers)
    // TArray<APlayerController*> VisiblePlayers;

    // void UpdateMeshVisibility(APlayerController* PlayerController);

    // Helper function to ensure visibility is correct at start
    // void InitializeVisibility();
    
};
