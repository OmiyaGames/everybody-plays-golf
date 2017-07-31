using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LudumDare39
{
    public interface IAnimatedMenu
    {
        bool IsVisible
        {
            get;
            set;
        }

        MenuCollection Parent
        {
            set;
        }
    }

    [RequireComponent(typeof(Animator))]
    public class BaseMenu : MonoBehaviour, IAnimatedMenu
    {
        public const string VisibleField = "Visible";

        public event System.Action<bool, bool> OnVisibleChanged;

        Animator animator;
        MenuCollection parent;

        public Animator Animator
        {
            get
            {
                if(animator == null)
                {
                    animator = GetComponent<Animator>();
                }
                return animator;
            }
        }

        public virtual bool IsVisible
        {
            get
            {
                return Animator.GetBool(VisibleField);
            }
            set
            {
                if (OnVisibleChanged != null)
                {
                    OnVisibleChanged(IsVisible, value);
                }
                Animator.SetBool(VisibleField, value);
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
    }
}
