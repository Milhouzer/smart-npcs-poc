#include "Game/BotAPISubsystem.h"

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