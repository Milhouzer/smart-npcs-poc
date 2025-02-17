// CaravanInterior.h
#pragma once
#include "Game/LevelSimulation/LevelSimulationData.h"
#include "Game/LevelSimulation/Interfaces/SimulatedLevel.h"

#include "CaravanInterior.generated.h"

USTRUCT(BlueprintType)
struct SNP_API FSavedObjectKey
{
    GENERATED_BODY()

    UPROPERTY(BlueprintReadWrite, EditAnywhere)
    int32 Id;
    
    UPROPERTY(BlueprintReadWrite, EditAnywhere)
    FString Name;
    
    UPROPERTY(BlueprintReadWrite, EditAnywhere)
    TSubclassOf<AActor> Type;
};

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

    virtual void GetLifetimeReplicatedProps(TArray<FLifetimeProperty>& OutLifetimeProps) const override;

    UPROPERTY(EditAnywhere, BlueprintReadWrite)
    TArray<FSavedObjectKey> ActorsReferences;

    UFUNCTION(BlueprintCallable)
    void SaveData();

    UFUNCTION(BlueprintCallable)
    void LoadData();
private:
    // Data associated to this simulation
    FLevelSimulationData* Data;
};
