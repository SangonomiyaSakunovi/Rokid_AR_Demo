﻿using Photons;
using SangoUtils.PhotonFusionHelpers.FusionCommons;
using SangoUtils.UnityDevelopToolKits.Loggers;
using SceneGameObjects;
using UnityEngine;
using UnityEngine.UI;

namespace PhotonUIPanels
{
    internal class FusionUIPanel_Game : FusionUIPanelBase
    {
        [SerializeField] protected Button _btn_playtest;
        [SerializeField] protected Button _btn_item0;
        [SerializeField] protected Button _btn_item1;
        [SerializeField] protected Button _btn_item2;

        private void OnEnable()
        {
            _btn_playtest.onClick.AddListener(delegate { OnBtnClicked(_btn_playtest.name); });
            _btn_item0.onClick.AddListener(delegate { OnBtnClicked(_btn_item0.name); });
            _btn_item1.onClick.AddListener(delegate { OnBtnClicked(_btn_item1.name); });
            _btn_item2.onClick.AddListener(delegate { OnBtnClicked(_btn_item2.name); });
        }

        private void OnDisable()
        {
            _btn_playtest.onClick.RemoveAllListeners();
            _btn_item0.onClick.RemoveAllListeners();
            _btn_item1.onClick.RemoveAllListeners();
            _btn_item2.onClick.RemoveAllListeners();
        }

        private void OnBtnClicked(string name)
        {
            if (name == _btn_playtest.name)
            {
                GameLogic.Instance.SetActiveObj<IGrabableObjBehaviour>(ObjectManager.ObjGrabableNames.Ghost_animation_Rokid);
            }
            else if (name == _btn_item0.name)
            {
                UnityLogger.Warning("Under development, please stay tuned!");
            }
            else if (name == _btn_item1.name)
            {
                UnityLogger.Warning("Under development, please stay tuned!");
            }
            else if (name == _btn_item2.name)
            {
                UnityLogger.Warning("Under development, please stay tuned!");
            }
        }
    }
}
