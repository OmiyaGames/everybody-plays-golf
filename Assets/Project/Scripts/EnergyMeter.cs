using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LudumDare39
{
    [RequireComponent(typeof(BaseMenu))]
    public class EnergyMeter : MonoBehaviour, IAnimatedMenu
    {
        [Serializable]
        public class Energy
        {
            [SerializeField]
            GameObject unit;

            GameObject childUnit;

            public Energy(Energy clone)
            {
                unit = Instantiate(clone.unit, clone.unit.transform.parent);
                unit.transform.localScale = Vector3.one;
                unit.transform.localRotation = Quaternion.identity;
            }

            public GameObject ChildUnit
            {
                get
                {
                    if(childUnit == null)
                    {
                        childUnit = unit.transform.GetChild(0).gameObject;
                    }
                    return childUnit;
                }
            }

            public bool IsOn
            {
                get
                {
                    return ChildUnit.activeSelf;
                }
                set
                {
                    ChildUnit.SetActive(value);
                }
            }
        }

        [SerializeField]
        Energy energyGraphic;

        BaseMenu menu;
        MenuCollection parent;
        Energy[] allGraphics;

        public bool IsVisible
        {
            get
            {
                return Menu.IsVisible;
            }
            set
            {
                Menu.IsVisible = value;
            }
        }

        public MenuCollection Parent
        {
            protected get
            {
                return parent;
            }
            set
            {
                parent = value;
            }
        }

        BaseMenu Menu
        {
            get
            {
                if(menu == null)
                {
                    menu = GetComponent<BaseMenu>();
                }
                return menu;
            }
        }

        public Energy[] AllGraphics
        {
            get
            {
                if(allGraphics == null)
                {
                    allGraphics = new Energy[AddPower.MaxEnergy];
                    allGraphics[0] = energyGraphic;
                    for(int index = 1; index < allGraphics.Length; ++index)
                    {
                        allGraphics[index] = new Energy(energyGraphic);
                    }
                }
                return allGraphics;
            }
        }

        // Update is called once per frame
        void Update()
        {
            if(Parent)
            {
                switch(Parent.CurrentState)
                {
                    case MenuCollection.MenuState.Start:
                    case MenuCollection.MenuState.EnterName:
                    case MenuCollection.MenuState.Connecting:
                    case MenuCollection.MenuState.Explanation:
                    case MenuCollection.MenuState.Goal:
                    case MenuCollection.MenuState.Controls:
                    case MenuCollection.MenuState.Congrats:
                        IsVisible = false;
                        break;
                    default:
                        IsVisible = true;
                        for (int index = 0; index < AllGraphics.Length; ++index)
                        {
                            AllGraphics[index].IsOn = (MenuCollection.Settings.CurrentEnergy > index);
                        }
                        break;
                }
            }
        }
    }
}
