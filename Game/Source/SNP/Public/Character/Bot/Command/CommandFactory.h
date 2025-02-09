// ReSharper disable once UnrealHeaderToolError
#pragma once

#include "CoreMinimal.h"
#include "CommandFactory.generated.h"

/**
 * CommandRegistry is a static class that maps command names to their corresponding bot functions.
 */
UCLASS(Blueprintable)
class SNP_API UCommandFactory : public UObject
{
	GENERATED_BODY()

public:
	template <typename CmdType, typename CmdDataType>
	static TSharedPtr<CmdType> CreateCmd(const CmdDataType& CmdData);
};


