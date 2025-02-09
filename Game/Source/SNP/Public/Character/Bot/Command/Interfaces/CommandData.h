#pragma once

#include "CoreMinimal.h"
#include "CommandData.generated.h"

static const FString CRAFT_COMMAND_NAME = "CRAFT";
static const FString TALK_COMMAND_NAME = "TALK";

/**
 * Struct that represents a command with its name and payload
*/
USTRUCT(BlueprintType)
struct SNP_API FCommandData 
{
	GENERATED_USTRUCT_BODY()

public:
	virtual ~FCommandData() = default;
	
	/** Name of the command */
	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Command")
	FString Name;
	
	FCommandData() {}
	
	FCommandData(const FString& InCommandName)
		: Name(InCommandName) {}
};

USTRUCT(BlueprintType)
struct SNP_API FCraftCommandData : public FCommandData 
{
	GENERATED_USTRUCT_BODY()
	
	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Command")
	FString ItemName;
	
	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Command")
	int Quantity = 0;

	FCraftCommandData() { }
	
	FCraftCommandData(const FString& InItemName, const int InQuantity) : FCommandData(CRAFT_COMMAND_NAME)
	{
		ItemName = InItemName;
		Quantity = InQuantity;
	}
};

USTRUCT(BlueprintType)
struct SNP_API FTalkCommandData : public FCommandData 
{
	GENERATED_USTRUCT_BODY()

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Command")
	FString Message;
	
	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Command")
	TArray<FString> POIs;
	
	FTalkCommandData() { }
	
	FTalkCommandData(const FString& InMessage, TArray<FString>& InPOIs) : FCommandData(TALK_COMMAND_NAME)
	{
		Message = InMessage;
		POIs = InPOIs;
	}
};
