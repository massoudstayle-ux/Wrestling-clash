#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Character.h"
#include "WrestlingClashCharacter.generated.h"

class UCameraComponent;
class USpringArmComponent;
class UStaticMeshComponent;

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

    UPROPERTY(VisibleAnywhere, Category = "Placeholder")
    TObjectPtr<UStaticMeshComponent> TorsoMesh;

    UPROPERTY(VisibleAnywhere, Category = "Placeholder")
    TObjectPtr<UStaticMeshComponent> HeadMesh;

    UPROPERTY(VisibleAnywhere, Category = "Placeholder")
    TObjectPtr<UStaticMeshComponent> LeftArmMesh;

    UPROPERTY(VisibleAnywhere, Category = "Placeholder")
    TObjectPtr<UStaticMeshComponent> RightArmMesh;

    UPROPERTY(VisibleAnywhere, Category = "Placeholder")
    TObjectPtr<UStaticMeshComponent> LeftLegMesh;

    UPROPERTY(VisibleAnywhere, Category = "Placeholder")
    TObjectPtr<UStaticMeshComponent> RightLegMesh;

    void MoveForward(float Value);
    void MoveRight(float Value);
};