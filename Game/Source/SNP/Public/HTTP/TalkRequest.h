#pragma once

#include "CoreMinimal.h"
#include "JsonObjectConverter.h"
#include "TalkRequest.generated.h"

USTRUCT(BlueprintType)
struct SNP_API FTalkRequest
{
	GENERATED_BODY()

	UPROPERTY(BlueprintReadWrite, EditAnywhere, meta = (DisplayName = "Message"))
	FString Message;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, meta = (DisplayName = "POIs"))
	TArray<FString> PointsOfInterest;
	
	FTalkRequest() {}

	FTalkRequest(const FString& InMessage, const TArray<FString>& InPOIs)
		: Message(InMessage), PointsOfInterest(InPOIs)
	{ }
};

USTRUCT(BlueprintType)
struct SNP_API FTalkResponse
{
	GENERATED_BODY()

	FTalkResponse() {}

	explicit FTalkResponse(const FString& InResponse) : Response(InResponse) {}

	UPROPERTY(BlueprintReadWrite, EditAnywhere)
	FString Response;
};

USTRUCT(BlueprintType)
struct SNP_API FSaveDataRequest
{
	GENERATED_BODY()

	UPROPERTY(BlueprintReadWrite, EditAnywhere, meta = (DisplayName = "PlayerId"))
	int PlayerId;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, meta = (DisplayName = "BinaryData"))
	TArray<uint8> BinaryData;

	FSaveDataRequest(): PlayerId(0)
	{
	}

	explicit FSaveDataRequest(const uint32& InPlayerId, const TArray<uint8>& InBinaryData)
		: PlayerId(InPlayerId), BinaryData(InBinaryData)
	{ }
};

USTRUCT(BlueprintType)
struct SNP_API FSaveDataResponse
{
	GENERATED_BODY()

	FSaveDataResponse() { }

	TArray<uint8> BinaryData;
};
