#include "WrestlingClashGameMode.h"
#include "WrestlingClashArena.h"
#include "WrestlingClashCharacter.h"
#include "Engine/World.h"

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