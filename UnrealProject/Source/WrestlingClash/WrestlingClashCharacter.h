#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Character.h"
#include "WrestlingClashCharacter.generated.h"

class UCameraComponent;
class USpringArmComponent;

UCLASS()
class WRESTLINGCLASH_API AWrestlingClashCharacter : public ACharacter
{
    GENERATED_BODY()

public:
    AWrestlingClashCharacter();

protected:
    virtual void SetupPlayerInputComponent(UInputComponent* PlayerInputComponent) override;

private:
    UPROPERTY(VisibleAnywhere, Category = "Camera")
    TObjectPtr<USpringArmComponent> CameraBoom;

    UPROPERTY(VisibleAnywhere, Category = "Camera")
    TObjectPtr<UCameraComponent> FollowCamera;

    void MoveForward(float Value);
    void MoveRight(float Value);
};