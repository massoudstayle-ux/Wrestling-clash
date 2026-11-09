#include "WrestlingClashGameMode.h"
#include "WrestlingClashArena.h"
#include "WrestlingClashCharacter.h"
#include "Engine/World.h"
#include "GameFramework/Controller.h"
#include "GameFramework/Pawn.h"

AWrestlingClashGameMode::AWrestlingClashGameMode()
{
    DefaultPawnClass = AWrestlingClashCharacter::StaticClass();
}

void AWrestlingClashGameMode::BeginPlay()
{
    Super::BeginPlay();

    if (UWorld* World = GetWorld())
    {
        World->SpawnActor<AWrestlingClashArena>(
            AWrestlingClashArena::StaticClass(),
            FVector::ZeroVector,
            FRotator::ZeroRotator);
    }
}

void AWrestlingClashGameMode::RestartPlayer(AController* NewPlayer)
{
    Super::RestartPlayer(NewPlayer);

    // The first prototype has no authored map or PlayerStart yet.
    // Place the wrestler above the ring surface to avoid starting inside it.
    if (NewPlayer && NewPlayer->GetPawn())
    {
        NewPlayer->GetPawn()->SetActorLocation(FVector(0.0f, 0.0f, 110.0f));
    }
}