#include "WrestlingClashGameMode.h"
#include "WrestlingClashArena.h"
#include "WrestlingClashCharacter.h"
#include "WrestlingClashHUD.h"
#include "Engine/World.h"
#include "GameFramework/Controller.h"
#include "GameFramework/Pawn.h"

AWrestlingClashGameMode::AWrestlingClashGameMode()
{
    DefaultPawnClass = AWrestlingClashCharacter::StaticClass();
    HUDClass = AWrestlingClashHUD::StaticClass();
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

        // Temporary stationary opponent for the first combat test.
        World->SpawnActor<AWrestlingClashCharacter>(
            AWrestlingClashCharacter::StaticClass(),
            FVector(150.0f, 0.0f, 110.0f),
            FRotator(0.0f, 180.0f, 0.0f));
    }
}

void AWrestlingClashGameMode::RestartPlayer(AController* NewPlayer)
{
    Super::RestartPlayer(NewPlayer);

    // The first prototype has no authored map or PlayerStart yet.
    if (NewPlayer && NewPlayer->GetPawn())
    {
        NewPlayer->GetPawn()->SetActorLocation(FVector(-150.0f, 0.0f, 110.0f));
        NewPlayer->SetControlRotation(FRotator::ZeroRotator);
    }
}