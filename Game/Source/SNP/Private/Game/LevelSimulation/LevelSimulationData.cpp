#include "Game/LevelSimulation/LevelSimulationData.h"
#include "Game/LevelSimulation/LevelSimulationAsset.h"

FLevelSimulationData::FLevelSimulationData(const ULevelSimulationAsset* DataAsset)
{
	BaseActor = DataAsset->BaseActor;
	SpawnTransform  = DataAsset->SpawnTransform;
	ExitTransform = DataAsset->ExitTransform;
}
