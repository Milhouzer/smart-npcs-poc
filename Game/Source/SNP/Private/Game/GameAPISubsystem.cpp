#include "Game/GameAPISubsystem.h"

#include "Game/Save/SaveState.h"
#include "HTTP/APIClient.h"

void UGameAPISubsystem::Initialize(FSubsystemCollectionBase& Collection)
{
	Super::Initialize(Collection);
    
	// Create the API client instance
	APIClient = NewObject<UAPIClient>(this);
    
	// TODO: Client is not initialized correctly
	// APIClient->Configure(TEXT("127.0.0.1"), 8080);
}

void UGameAPISubsystem::Deinitialize()
{
	// if (APIClient)
	// {
	// 	APIClient->ConditionalBeginDestroy();
	// 	// TODO: WARNING : There is an error here !!
	// 	// APIClient = nullptr;
	// }
    
	Super::Deinitialize();
}

void UGameAPISubsystem::SendTalkCommand(const FTalkRequest& Data,
	TFunction<void(const FTalkResponse&, bool)> Callback)
{
	if(APIClient == nullptr)
	{
		// Call the callback with failure
		FTalkResponse EmptyResponse = FTalkResponse();
		EmptyResponse.Response = "Service unavailable";
		Callback(EmptyResponse, false);
		return;
	}

	APIClient->Post("bot/talk", Data, Callback);
}

void UGameAPISubsystem::SendSaveCommand(const FSaveData& Data)
{
	if (APIClient == nullptr)
	{
		UE_LOG(LogTemp, Warning, TEXT("APIClient is null. Save command failed."));
		return;
	}

	// Ensure the lambda matches the expected TResponse
	APIClient->Post<FSaveData, FLoadedData>("player/save", Data,
		[this](const FLoadedData& Response, const bool Success)
		{
			if (!Success)
			{
				UE_LOG(LogTemp, Warning, TEXT("Failed to send save command to API."));
			}
		});
}

void UGameAPISubsystem::SendSaveCommand(const FSaveDataArray& Data)
{
	if (APIClient == nullptr)
	{
		UE_LOG(LogTemp, Warning, TEXT("APIClient is null. Save command failed."));
		return;
	}

	// Ensure the lambda matches the expected TResponse
	APIClient->Post<FSaveDataArray, FLoadedData>("player/save", Data,
		[this](const FLoadedData& Response, const bool Success)
		{
			if (!Success)
			{
				UE_LOG(LogTemp, Warning, TEXT("Failed to send save command to API."));
			}
		});
}

FLoadedData UGameAPISubsystem::LoadData(int PlayerId,
    TFunction<void(const FLoadedDataArray&, bool)> Callback)
{
	if (APIClient == nullptr)
	{
		UE_LOG(LogTemp, Warning, TEXT("APIClient is null. Save command failed."));
		FLoadedData EmptyResponse = FLoadedData();
		return EmptyResponse;
	}

	const FString URL = FString::Printf(TEXT("player/load?playerId=%d"), PlayerId);
	APIClient->Get<FLoadedDataArray>(URL, Callback);
	
	FLoadedData EmptyResponse = FLoadedData();
	return EmptyResponse;
}

