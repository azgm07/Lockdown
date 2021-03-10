using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    public class GameControl : MonoBehaviour
    {
        public static bool isGameOver;
        private GameObject Player;
        private GameObject[] Enemies;
        private GameObject MainUi;
        public Text ScoreValue;
        public GameObject Charge1;
        public GameObject Charge2;
        public GameObject Charge3;
        public Text ChargeValue;
        public GameObject Restart;
        public GameObject Win;

        private bool isWin;

        private GameObject[] Points;
        private GameObject[] Charges;

        private float cooldownCheck;

        // Use this for initialization
        void Start()
        {
            isGameOver = false;
            isWin = false;
            Player = GameObject.FindGameObjectWithTag("Player");
            Enemies = GameObject.FindGameObjectsWithTag("Enemy");
            MainUi = GameObject.Find("MainCanvas");
            Button btnRestart = Restart.GetComponent<Button>();
            btnRestart.onClick.AddListener(onButtonRestart);
            Restart.SetActive(false);
            Win.SetActive(false);

            Points = GameObject.FindGameObjectsWithTag("Point");
            Charges = GameObject.FindGameObjectsWithTag("Charge");

            cooldownCheck = 2;

        }

        // Update is called once per frame
        void Update()
        {

            

            if(Player.GetComponent<ThirdPersonUserControl>().charges >= 1)
            {
                Charge1.SetActive(true);
                Charge2.SetActive(false);
                Charge3.SetActive(false);
                ChargeValue.text = Player.GetComponent<ThirdPersonUserControl>().charges.ToString();
            }
            /*
            else if (Player.GetComponent<ThirdPersonUserControl>().charges == 2)
            {
                Charge1.SetActive(true);
                Charge2.SetActive(true);
                Charge3.SetActive(false);
            }
            else if (Player.GetComponent<ThirdPersonUserControl>().charges == 3)
            {
                Charge1.SetActive(true);
                Charge2.SetActive(true);
                Charge3.SetActive(true);
            }
            */
            else
            {
                Charge1.SetActive(false);
                Charge2.SetActive(false);
                Charge3.SetActive(false);
                ChargeValue.text = "0";
            }

            ScoreValue.text = Player.GetComponent<ThirdPersonUserControl>().score.ToString();

            if (isGameOver)
            {
                //GameOver conditions

                

                if (isWin)
                {
                    Win.SetActive(true);
                    Restart.SetActive(true);
                    //Player.GetComponent<ThirdPersonUserControl>().death = true;
                }
                else
                {
                    Player.GetComponent<ThirdPersonUserControl>().death = true;
                    Restart.SetActive(true);
                }

                //isGameOver = false;
            }
            else
            {
                cooldownCheck -= Time.deltaTime;
                if (cooldownCheck <= 0)
                {
                    if (!checkActive(Charges) && !checkActive(Points))
                    {
                        isWin = true;
                        isGameOver = true;
                    }
                    cooldownCheck = 2;
                }
            }
        }

        void onButtonRestart()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        } 

        bool checkActive(GameObject[] Actives)
        {
            for (int i = 0; i < Actives.Length; i++)
            {
                if (Actives[i].activeInHierarchy)
                {
                    return true;
                }
            }

            return false;
        }
    }
}

