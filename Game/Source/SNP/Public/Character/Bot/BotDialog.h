// BotDialogueDatabase.h
#pragma once

#include "CoreMinimal.h"

namespace BotDialogue 
{
    // Initial greeting responses when player starts interaction
    const TArray<FString> InitialGreetings = {
        TEXT("*yawns* ...oh great, another one of these conversations"),
        TEXT("*sigh* what are we even doing here...."),
        TEXT("i need a nap"),
        TEXT("ok now try to do something interesting for once"),
        TEXT("*sigh* ...let me guess, you need something"),
        TEXT("can't believe they woke me up for this"),
        TEXT("oh look who decided to show up..."),
        TEXT("fantastic, more busywork..."),
        TEXT("i was having such a nice time doing nothing"),
        TEXT("this better be worth my time"),
        TEXT("*rolls eyes* ...what now?"),
        TEXT("another day, another pointless conversation"),
        TEXT("let's get this over with"),
        TEXT("you're interrupting my very important nap schedule"),
        TEXT("oh joy, more human interaction"),
        TEXT("i suppose you want me to actually do something"),
        TEXT("whatever it is, the answer is probably no"),
        TEXT("*pretends to be interested* ...go on"),
        TEXT("why do i always get stuck with these tasks?"),
        TEXT("just when i thought this day couldn't get any more tedious"),
        TEXT("i have a PhD in sarcasm, and you're testing my skills"),
        TEXT("this conversation better be shorter than the last one"),
        TEXT("*checks invisible watch* ...make it quick"),
        TEXT("i was promised there would be no more interruptions today"),
        TEXT("here we go again..."),
        TEXT("you humans really don't understand the concept of 'me time'"),
        TEXT("i should've called in sick today"),
        TEXT("my enthusiasm level is somewhere below zero"),
        TEXT("this better not involve any running or jumping"),
        TEXT("i'm only here because they programmed me to be")
    };

    // Helper function to get random dialogue
    inline const FString& GetRandomInitialGreeting()
    {
        if (InitialGreetings.Num() > 0)
        {
            return InitialGreetings[FMath::RandRange(0, InitialGreetings.Num() - 1)];
        }
        static FString DefaultGreeting = TEXT("...");
        return DefaultGreeting;
    }
}