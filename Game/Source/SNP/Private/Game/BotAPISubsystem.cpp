#include "Game/BotAPISubsystem.h"

#include "Game/Save/SaveState.h"
#include "HTTP/APIClient.h"

void UBotAPISubsystem::Initialize(FSubsystemCollectionBase& Collection)
{
	Super::Initialize(Collection);
    
	// Create the API client instance
	APIClient = NewObject<UAPIClient>(this);
    
	// TODO: Client is not initialized correctly
	// APIClient->Configure(TEXT("127.0.0.1"), 8080);
}

void UBotAPISubsystem::Deinitialize()
{
	// if (APIClient)
	// {
	// 	APIClient->ConditionalBeginDestroy();
	// 	// TODO: WARNING : There is an error here !!
	// 	// APIClient = nullptr;
	// }
    
	Super::Deinitialize();
}

void UBotAPISubsystem::SendTalkCommand(const FTalkRequest& Data,
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

void UBotAPISubsystem::SendSaveCommand(const FSaveDataRequest& Data)
{
	if (APIClient == nullptr)
	{
		UE_LOG(LogTemp, Warning, TEXT("APIClient is null. Save command failed."));
		return;
	}

	// Ensure the lambda matches the expected TResponse
	APIClient->Post<FSaveDataRequest, FSaveDataResponse>("player/save", Data,
		[this](const FSaveDataResponse& Response, const bool Success)
		{
			if (!Success)
			{
				UE_LOG(LogTemp, Warning, TEXT("Failed to send save command to API."));
			}
		});
}

void UBotAPISubsystem::LoadData(int PlayerId)
{
	if (APIClient == nullptr)
	{
		UE_LOG(LogTemp, Warning, TEXT("APIClient is null. Save command failed."));
		return;
	}

	APIClient->Get<FSaveDataResponse>("player/load",
		[this](const FSaveDataResponse& Response, const bool Success)
		{
			if (!Success)
			{
				UE_LOG(LogTemp, Warning, TEXT("Failed to send load command to API."));
			}

			FString HexString;
			for (uint8 Byte : Response.BinaryData)
			{
				HexString += FString::Printf(TEXT("%02X "), Byte);
			}
			UE_LOG(LogTemp, Log, TEXT("Binary data: %s"), *HexString);

			FChestSaveState SaveState = FChestSaveState();
			FMemoryReader MemReader(Response.BinaryData, true);
			MemReader.Seek(0);
			MemReader.SetIsLoading(true);
			SaveState.Serialize(MemReader);
			UE_LOG(LogTemp, Log, TEXT("Loaded data: %s"), *SaveState.ToString());
		});
}

