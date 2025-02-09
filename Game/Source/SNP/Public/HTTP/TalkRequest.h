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
	{
		FString POIsString = FString::Join(InPOIs, TEXT(", "));
	}
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
