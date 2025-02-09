#include "Game/LevelSimulation/LevelSimulationSubsystem.h"

#include "Algo/AnyOf.h"
#include "Game/LevelSimulation/LevelSimulationData.h"
#include "Game/LevelSimulation/Interfaces/LevelSimulator.h"
#include "Game/LevelSimulation/Interfaces/SimulatedLevel.h"
#include "Utils/Utility.h"

void ULevelsManager::Initialize(FSubsystemCollectionBase& Collection)
{
	Super::Initialize(Collection);
}

void ULevelsManager::Deinitialize()
{
	Super::Deinitialize();
}

TScriptInterface<ISimulatedLevel> ULevelsManager::AddSimulation(const ILevelSimulator* Simulator, FLevelSimulationData* Data)
{
	if(!VALIDATE_PARAMS(Simulator, Data)) { return nullptr; }

	FVector Location = Data->SpawnTransform.GetLocation() + FVector(5000 * LevelSimulations.size(), 0, -100);
	Data->SpawnTransform.SetLocation(Location);
	const FRotator RelativeRotation = FRotator::ZeroRotator;
	AActor* Simulation = GetWorld()->SpawnActor<AActor>(Data->BaseActor, Data->SpawnTransform.GetLocation(), RelativeRotation);
	ISimulatedLevel* Level = Cast<ISimulatedLevel>(Simulation);
	Level->Init(Data);
	LevelSimulations.push_back(Level);
	UE_LOG(LogTemp, Log, TEXT("Simulation added"));
	
	TScriptInterface<ISimulatedLevel> ScriptInterface;
	ScriptInterface.SetObject(Simulation);
	ScriptInterface.SetInterface(Level);
	return ScriptInterface;
}

bool ULevelsManager::RemoveSimulation(const ILevelSimulator& Simulator)
{
	// Find simulation with playercontroller as owner and stop it.
	return false;
}

bool ULevelsManager::Render(ILevelSimulator* Simulator, APlayerController* PlayerController)
{
	if(!VALIDATE_PARAMS(Simulator, PlayerController)) { 
		UE_LOG(LogTemp, Log, TEXT("ULevelsManager::Render(): cannot render simulation"));
		return false;
	}
	
	SimulatedLevels.FindOrAdd(PlayerController).AddUnique(Simulator->GetSimulatedLevel().GetInterface());
	
	UE_LOG(LogTemp, Log, TEXT("ULevelsManager::Render(): %p is now rendering for %s"),
		   Simulator, *PlayerController->GetName());

	return true;
}

bool ULevelsManager::StopRender(ILevelSimulator* Simulator, APlayerController* PlayerController)
{
	if(!VALIDATE_PARAMS(Simulator, PlayerController)) { return false; }
	
	bool bExists = SimulatedLevels.Contains(PlayerController) && 
		Algo::AnyOf(SimulatedLevels[PlayerController], [Simulator](const ISimulatedLevel* Sim)
		{
			return Sim == Simulator->GetSimulatedLevel().GetInterface();
		});

	if(!bExists)
	{
		UE_LOG(LogTemp, Warning, TEXT("Simulation rendering is already stopped for %s"), *PlayerController->GetName());
		return false;
	}

	if (TArray<ISimulatedLevel*>* SimArray = SimulatedLevels.Find(PlayerController))
	{
		SimArray->Remove(Simulator->GetSimulatedLevel().GetInterface());

		if (SimArray->Num() == 0)
		{
			SimulatedLevels.Remove(PlayerController);
		}
	}

	return true;
}

// ISimulatedLevel* ULevelsManager::GetSimulation(ILevelSimulator* Simulator)
// {
// 	if(!VALIDATE_PARAMS(Simulator)) { return nullptr; }
// 	
// 	auto It = std::find_if(LevelSimulations.begin(), LevelSimulations.end(),
// 		[Simulator](FLevelSimulation* Simulation)
// 	{
// 		return Simulation->IsOwner(Simulator);
// 	});
//
// 	return (It != LevelSimulations.end()) ? *It : nullptr;
// }

// void ULevelsManager::Tick(float DeltaTime)
// {
// 	std::for_each(LevelSimulations.begin(), LevelSimulations.end(),
// 		[DeltaTime, this](FLevelSimulation* Simulation)
// 	{
// 		if (Simulation)
// 		{
// 			Simulation->Simulate(DeltaTime);
// 		}
// 	});
// }

bool ULevelsManager::IsRendering(const ISimulatedLevel* Simulation, const APlayerController* PlayerController)
{
	return SimulatedLevels.Contains(PlayerController) && 
		Algo::AnyOf(SimulatedLevels[PlayerController], [Simulation](const ISimulatedLevel* Sim)
		{
			return Sim == Simulation;
		});;
}
