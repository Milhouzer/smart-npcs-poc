#pragma once

#include "CoreMinimal.h"
#include "Game/LevelSimulation/LevelSimulationAsset.h"
#include "GameFramework/Actor.h"
#include "Game/LevelSimulation/Interfaces/LevelSimulator.h"
#include "SimulationRenderer.generated.h"


class ISimulatedLevel;

UCLASS(ClassGroup=(Custom), meta=(BlueprintSpawnableComponent))
class SNP_API USimulationRenderer : public USceneComponent, public ILevelSimulator
{
	GENERATED_BODY()

public:
	// Asset used to create the simulation
	UPROPERTY(EditAnywhere, BlueprintReadWrite, Category = "Simulation")
	ULevelSimulationAsset* SimulationDataAsset;
	// Asset used to create the simulation
	
	UFUNCTION(BlueprintCallable)
	virtual void Init() override;
	
	UFUNCTION(BlueprintCallable)
	virtual void Render(AActor* Actor) override;
	
	UFUNCTION(BlueprintCallable)
	virtual void StopRender(AActor* Actor) override;

	UFUNCTION(BlueprintCallable)
	virtual TScriptInterface<ISimulatedLevel> GetSimulatedLevel() override
	{
		return Simulation;
	}
	
protected:
	virtual void BeginPlay() override;
	virtual void GetLifetimeReplicatedProps(TArray<FLifetimeProperty>& OutLifetimeProps) const override;

	TScriptInterface<ISimulatedLevel> Simulation;
};