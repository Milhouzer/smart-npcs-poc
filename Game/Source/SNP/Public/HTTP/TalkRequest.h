#pragma once

#include "CoreMinimal.h"
#include "JsonObjectConverter.h"
#include "TalkRequest.generated.h"


/************************/
/*			TALK		*/
/************************/

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

/************************/
/*		SAVE DATA		*/
/************************/

USTRUCT(BlueprintType)
struct SNP_API FSaveData
{
	GENERATED_BODY()

	UPROPERTY(BlueprintReadWrite, EditAnywhere, meta = (DisplayName = "PlayerId"))
	int PlayerId;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, meta = (DisplayName = "BinaryData"))
	FString Base64Data;

	FSaveData(): PlayerId(0)
	{
	}

	explicit FSaveData(const uint32& InPlayerId, const FString& InBase64Data)
		: PlayerId(InPlayerId), Base64Data(InBase64Data)
	{ }
};

USTRUCT(BlueprintType)
struct SNP_API FSaveDataArray
{
	GENERATED_BODY()

	UPROPERTY(BlueprintReadWrite, EditAnywhere, meta = (DisplayName = "DataArray"))
	TArray<FSaveData> SaveData;
	
	FSaveDataArray() {}
};

USTRUCT(BlueprintType)
struct SNP_API FSaveResponse
{
	GENERATED_BODY()
};

/************************/
/*		LOAD DATA		*/
/************************/

USTRUCT(BlueprintType)
struct SNP_API FLoadedData
{
	GENERATED_BODY()

	FLoadedData() { }
		
	UPROPERTY()
	FString Base64Data;

	TArray<uint8> GetBinaryData() const
	{
		TArray<uint8> BinaryData;
		if(!FBase64::Decode(Base64Data, BinaryData))
		{
			return TArray<uint8>();
		}
		
		return BinaryData;
	}
	
	FString ToString() const
	{
		return FString::Printf(TEXT("Base64Data: %s"), *Base64Data);
	}
};

USTRUCT(BlueprintType)
struct SNP_API FLoadedDataArray
{
	GENERATED_BODY()

	UPROPERTY()
	TArray<FLoadedData> DataArray;
	
	FLoadedDataArray() {}

	FString ToString() const
	{
		FString Result;
		for (const FLoadedData& Data : DataArray)
		{
			Result += Data.ToString() + TEXT(", ");
		}
		return Result.IsEmpty() ? TEXT("Empty") : Result;
	}
};

USTRUCT(BlueprintType)
struct SNP_API FLoadResponse
{
	GENERATED_BODY()
};

