// GENERATED AUTOMATICALLY FROM 'Assets/MicroCars/MicroCarsInputActions.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace MicroCars.Inputs
{
    public class @MicroCarsInputsActions : IInputActionCollection, IDisposable
    {
        public InputActionAsset asset { get; }
        public @MicroCarsInputsActions()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""MicroCarsInputActions"",
    ""maps"": [
        {
            ""name"": ""Gameplay"",
            ""id"": ""9b10df32-8d7b-405a-b87e-f8a0a00cbe1a"",
            ""actions"": [
                {
                    ""name"": ""Accel"",
                    ""type"": ""Value"",
                    ""id"": ""9e7c9a87-23d6-4fdb-aff4-521c221dad7e"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Brake"",
                    ""type"": ""Value"",
                    ""id"": ""21a94707-e195-493b-b633-bdfddeaff57a"",
                    ""expectedControlType"": """",
                    ""processors"": ""Clamp(max=1)"",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Steer"",
                    ""type"": ""Value"",
                    ""id"": ""7c06a990-a6bb-4adb-8c33-1889aef6fc65"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""HandBrake"",
                    ""type"": ""Value"",
                    ""id"": ""92a85fe7-f680-41e2-880d-e226005a0da7"",
                    ""expectedControlType"": """",
                    ""processors"": ""Clamp(max=1)"",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Boost"",
                    ""type"": ""Value"",
                    ""id"": ""f82eaa86-3fdb-4727-a26a-6cbe6a46d86c"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": ""Clamp(max=1)"",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""Keyboard axis"",
                    ""id"": ""65a8d3bf-ed0f-422d-b1f3-c1e468492c33"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Steer"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""a05dc447-0a14-4e47-b0da-a2c950f8a4ac"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Steer"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""c9b1cbae-9bac-4073-9af7-d72f3e1319c8"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Steer"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Keyboard arrows axis"",
                    ""id"": ""a690768d-6eef-4dcf-9d1a-776eaf4f0430"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Steer"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""4ccf7903-1062-4469-abf2-3747bc2b1679"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Steer"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""791a712b-a320-44d0-a996-0731eca92ef2"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Steer"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Gamepad axis"",
                    ""id"": ""71f3e542-94f1-4d4e-94bb-92139a90a912"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Steer"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""c3787f18-3a98-497b-a0ea-31c154ed703a"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Steer"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""96cb04b8-3654-41b1-8215-1beefe1d9f09"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Steer"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Keyboard axis"",
                    ""id"": ""0de7e3aa-87c4-49a3-89dd-ef403276973b"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Accel"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""Positive"",
                    ""id"": ""c5712363-3729-4579-b3ce-e9785fff34f1"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Accel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Negative"",
                    ""id"": ""9ba0c9ae-3075-4aa4-938e-cf485d55770a"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Accel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Keyboard arrows axis"",
                    ""id"": ""0756bf4e-cae7-47d3-bc7f-8530d4c84104"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Accel"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""56e64618-2884-43a1-b91c-41e08e872c93"",
                    ""path"": ""<Keyboard>/upArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Accel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""645cceb8-b08b-4e7f-97d6-b48ed0d7c005"",
                    ""path"": ""<Keyboard>/downArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Accel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Gamepad axis"",
                    ""id"": ""5cefa54b-e446-484f-9f7f-0664f9c7c249"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Accel"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""589eedf5-3e4e-4a36-ae1b-97df0ed482ea"",
                    ""path"": ""<Gamepad>/rightTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Accel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""86592eb9-0f0e-4925-9738-894da9fe40ce"",
                    ""path"": ""<Gamepad>/leftTrigger"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Accel"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""5d19cf05-77a9-4b3a-9e1a-d28e40839515"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""HandBrake"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3739567e-e6ec-4876-86e8-03d25b4fd3e0"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""HandBrake"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b17de2d6-aa7b-4919-aef8-b1967b1a3503"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Boost"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ebe679c2-bd87-4b02-9f9f-f22a5a1a83f5"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Boost"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""a301fb0d-64e0-474a-9ec9-c1af554fd364"",
                    ""path"": ""<Keyboard>/ctrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Brake"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f3c04ff1-279c-4952-bf07-7cec976826a8"",
                    ""path"": ""<Gamepad>/buttonWest"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Brake"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
            // Gameplay
            m_Gameplay = asset.FindActionMap("Gameplay", throwIfNotFound: true);
            m_Gameplay_Accel = m_Gameplay.FindAction("Accel", throwIfNotFound: true);
            m_Gameplay_Brake = m_Gameplay.FindAction("Brake", throwIfNotFound: true);
            m_Gameplay_Steer = m_Gameplay.FindAction("Steer", throwIfNotFound: true);
            m_Gameplay_HandBrake = m_Gameplay.FindAction("HandBrake", throwIfNotFound: true);
            m_Gameplay_Boost = m_Gameplay.FindAction("Boost", throwIfNotFound: true);
        }

        public void Dispose()
        {
            UnityEngine.Object.Destroy(asset);
        }

        public InputBinding? bindingMask
        {
            get => asset.bindingMask;
            set => asset.bindingMask = value;
        }

        public ReadOnlyArray<InputDevice>? devices
        {
            get => asset.devices;
            set => asset.devices = value;
        }

        public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

        public bool Contains(InputAction action)
        {
            return asset.Contains(action);
        }

        public IEnumerator<InputAction> GetEnumerator()
        {
            return asset.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Enable()
        {
            asset.Enable();
        }

        public void Disable()
        {
            asset.Disable();
        }

        // Gameplay
        private readonly InputActionMap m_Gameplay;
        private IGameplayActions m_GameplayActionsCallbackInterface;
        private readonly InputAction m_Gameplay_Accel;
        private readonly InputAction m_Gameplay_Brake;
        private readonly InputAction m_Gameplay_Steer;
        private readonly InputAction m_Gameplay_HandBrake;
        private readonly InputAction m_Gameplay_Boost;
        public struct GameplayActions
        {
            private @MicroCarsInputsActions m_Wrapper;
            public GameplayActions(@MicroCarsInputsActions wrapper) { m_Wrapper = wrapper; }
            public InputAction @Accel => m_Wrapper.m_Gameplay_Accel;
            public InputAction @Brake => m_Wrapper.m_Gameplay_Brake;
            public InputAction @Steer => m_Wrapper.m_Gameplay_Steer;
            public InputAction @HandBrake => m_Wrapper.m_Gameplay_HandBrake;
            public InputAction @Boost => m_Wrapper.m_Gameplay_Boost;
            public InputActionMap Get() { return m_Wrapper.m_Gameplay; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(GameplayActions set) { return set.Get(); }
            public void SetCallbacks(IGameplayActions instance)
            {
                if (m_Wrapper.m_GameplayActionsCallbackInterface != null)
                {
                    @Accel.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAccel;
                    @Accel.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAccel;
                    @Accel.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnAccel;
                    @Brake.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnBrake;
                    @Brake.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnBrake;
                    @Brake.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnBrake;
                    @Steer.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnSteer;
                    @Steer.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnSteer;
                    @Steer.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnSteer;
                    @HandBrake.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnHandBrake;
                    @HandBrake.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnHandBrake;
                    @HandBrake.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnHandBrake;
                    @Boost.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnBoost;
                    @Boost.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnBoost;
                    @Boost.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnBoost;
                }
                m_Wrapper.m_GameplayActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @Accel.started += instance.OnAccel;
                    @Accel.performed += instance.OnAccel;
                    @Accel.canceled += instance.OnAccel;
                    @Brake.started += instance.OnBrake;
                    @Brake.performed += instance.OnBrake;
                    @Brake.canceled += instance.OnBrake;
                    @Steer.started += instance.OnSteer;
                    @Steer.performed += instance.OnSteer;
                    @Steer.canceled += instance.OnSteer;
                    @HandBrake.started += instance.OnHandBrake;
                    @HandBrake.performed += instance.OnHandBrake;
                    @HandBrake.canceled += instance.OnHandBrake;
                    @Boost.started += instance.OnBoost;
                    @Boost.performed += instance.OnBoost;
                    @Boost.canceled += instance.OnBoost;
                }
            }
        }
        public GameplayActions @Gameplay => new GameplayActions(this);
        public interface IGameplayActions
        {
            void OnAccel(InputAction.CallbackContext context);
            void OnBrake(InputAction.CallbackContext context);
            void OnSteer(InputAction.CallbackContext context);
            void OnHandBrake(InputAction.CallbackContext context);
            void OnBoost(InputAction.CallbackContext context);
        }
    }
}
