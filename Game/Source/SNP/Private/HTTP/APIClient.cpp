// APIClient.cpp
#include "HTTP/APIClient.h"
    
UAPIClient::UAPIClient()
    : BaseAddress(TEXT("http://127.0.0.1"))
    , Port(8080)
{
}   

void UAPIClient::Configure(const FString& InAddress, int32 InPort)
{
    BaseAddress = InAddress;
    Port = InPort;
}

TSharedRef<IHttpRequest> UAPIClient::CreateRequest(const FString& Endpoint, const FString& Verb)
{
    TSharedRef<IHttpRequest> Request = FHttpModule::Get().CreateRequest();
    Request->SetVerb(Verb);
	// TODO: Client is not initialized correctly, this is hard coded for now
    FString URL = FString::Printf(TEXT("http://127.0.0.1:8080/%s"), *Endpoint);
    Request->SetURL(URL); 
    Request->SetHeader(TEXT("Content-Type"), TEXT("application/json"));
    
    // Optional logging for debugging
    UE_LOG(LogTemp, Verbose, TEXT("Created request with URL: %s"), *Request->GetURL());
    
    return Request;
}
