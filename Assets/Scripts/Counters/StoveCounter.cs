using System;
using Unity.VisualScripting;
using UnityEngine;

public class StoveCounter : BaseCounter, IHasProgress {

    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs : EventArgs {
        public State state;
    }

    public enum State {
        Idle,
        Cooking,
        Cooked,
        Burned,
    }

    [SerializeField] private StoveRecipeSO[] fryingRecipeSOArray;
    [SerializeField] private StoveBurningRecipeSO[] stoveBurningRecipeSOArray;


    private State state;
    private float stoveTimer;
    private float burningTimer;
    private StoveRecipeSO stoveRecipeSO;
    private StoveBurningRecipeSO stoveBurningRecipeSO;

    private void Start() {
        state = State.Idle;
    }

    private void Update() {
        switch (state) {
            case State.Idle:
                break;
            case State.Cooking:
                stoveTimer += Time.deltaTime;
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                    progressNormalized = (float)stoveTimer / stoveRecipeSO.stoveTimerMax
                });

                if (stoveTimer > stoveRecipeSO.stoveTimerMax) {
                    // Cooked
                    GetKitchenObject().DestroySelf();

                    KitchenObject.SpawnKitchenObject(stoveRecipeSO.output, this);

                    state = State.Cooked;
                    burningTimer = 0f;
                    stoveBurningRecipeSO = GetStoveBurningRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
                        state = state
                    });
                }
                break;
            case State.Cooked:
                burningTimer += Time.deltaTime;

                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                    progressNormalized = (float)burningTimer / stoveBurningRecipeSO.burningTimerMax
                });

                if (burningTimer > stoveBurningRecipeSO.burningTimerMax) {
                    // Cooked
                    GetKitchenObject().DestroySelf();

                    KitchenObject.SpawnKitchenObject(stoveBurningRecipeSO.output, this);

                    state = State.Burned;

                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
                        state = state
                    });

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                        progressNormalized = 0f
                    });
                }
                break;
            case State.Burned:
                break;
        }

    }

    public override void Interact(Player player) {
        if (!HasKitchenObject()) {
            if (player.HasKitchenObject()) {
                // If I want to be able to place uncutable items on the cutting counter, remove the check below (if statement with hasrecipeinput)
                if (HasRecipeWithInput(player.GetKitchenObject().GetKitchenObjectSO())) {
                    //Player is carrying something that can be grilled
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    stoveRecipeSO = GetStoveRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                    state = State.Cooking;
                    stoveTimer = 0f;

                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
                        state = state
                    });

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                        progressNormalized = (float)stoveTimer / stoveRecipeSO.stoveTimerMax
                    });
                }
            }
            else {
                // Player is not carrying anything
            }
        }
        else {
            if (player.HasKitchenObject()) {
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                    // Player is holding a plate
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO())) {
                        GetKitchenObject().DestroySelf();

                        state = State.Idle;

                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
                            state = state
                        });

                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                            progressNormalized = 0f
                        });
                    }
                }
            }
            else {
                GetKitchenObject().SetKitchenObjectParent(player);

                state = State.Idle;

                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
                    state = state
                });

                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                    progressNormalized = 0f
                });
            }
        }
    }

    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO) {
        StoveRecipeSO stoveRecipeSO = GetStoveRecipeSOWithInput(inputKitchenObjectSO);
        return stoveRecipeSO != null;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO) {
        StoveRecipeSO stoveRecipeSO = GetStoveRecipeSOWithInput(inputKitchenObjectSO);
        if (stoveRecipeSO != null) {
            return stoveRecipeSO.output;
        }
        else {
            return null;
        }
    }

    private StoveRecipeSO GetStoveRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO) {
        foreach (StoveRecipeSO stoveRecipeSO in fryingRecipeSOArray) {
            if (stoveRecipeSO.input == inputKitchenObjectSO) {
                return stoveRecipeSO;
            }
        }
        return null;
    }

    private StoveBurningRecipeSO GetStoveBurningRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO) {
        foreach (StoveBurningRecipeSO stoveBurningRecipeSO in stoveBurningRecipeSOArray) {
            if (stoveBurningRecipeSO.input == inputKitchenObjectSO) {
                return stoveBurningRecipeSO;
            }
        }
        return null;
    }
}
