#pragma once
#include "SaveState.h"

#include "Saveable.generated.h"

UINTERFACE(Blueprintable)
class SNP_API USaveable : public UInterface
{
	GENERATED_BODY()
};

class SNP_API ISaveable
{
	GENERATED_BODY()

	virtual TArray<uint8> GetSaveState() = 0;
	virtual void LoadSaveState(FMemoryReader SaveState) = 0;
};