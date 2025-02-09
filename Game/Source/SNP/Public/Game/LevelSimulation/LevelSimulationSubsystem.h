#pragma once

#include "LevelSimulationData.h"
#include "Interfaces/LevelSimulator.h"
#include "LevelSimulationSubsystem.generated.h"

class ISimulatedLevel;
class FLevelSimulation;

UCLASS()
class ULevelsManager : public UGameInstanceSubsystem
{
	GENERATED_BODY()

public:
	// Initialize and shutdown
	virtual void Initialize(FSubsystemCollectionBase& Collection) override;
	virtual void Deinitialize() override;
	
	TScriptInterface<ISimulatedLevel> AddSimulation(const ILevelSimulator* Simulator, FLevelSimulationData* Data);
	bool RemoveSimulation(const ILevelSimulator& Simulator);

	bool Render(ILevelSimulator* Simulator, APlayerController* PlayerController);
	bool StopRender(ILevelSimulator* Simulator, APlayerController* PlayerController);

	// void Tick(float DeltaTime);

	bool IsRendering(const ISimulatedLevel* Simulation, const APlayerController* PlayerController);

protected:
	// ISimulatedLevel* GetSimulation(ILevelSimulator* Simulator);
	
	std::vector<ISimulatedLevel*> LevelSimulations;
	TMap<APlayerController*, TArray<ISimulatedLevel*>> SimulatedLevels;
};
