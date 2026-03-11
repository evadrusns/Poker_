using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Poker
{
    class Program
    {
        // -----------------------
        // DECLARATION DES DONNEES
        // -----------------------
        // Importation des DL (librairies de code) permettant de gérer les couleurs en mode console
        [DllImport("kernel32.dll")]
        public static extern bool SetConsoleTextAttribute(IntPtr hConsoleOutput, int wAttributes);
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetStdHandle(uint nStdHandle);
        static uint STD_OUTPUT_HANDLE = 0xfffffff5;
        static IntPtr hConsole = GetStdHandle(STD_OUTPUT_HANDLE);
        // Pour utiliser la fonction C 'getchar()' : sasie d'un caractère
        [DllImport("msvcrt")]
        static extern int _getch();

        //-------------------
        // TYPES DE DONNEES
        //-------------------

        // Fin du jeu
        public static bool fin = false;

        // Codes COULEUR
        public enum couleur { VERT = 10, ROUGE = 12, JAUNE = 14, BLANC = 15, NOIRE = 0, ROUGESURBLANC = 252, NOIRESURBLANC = 240 };

        // Coordonnées pour l'affichage
        public struct coordonnees
        {
            public int x;
            public int y;
        }

        // Une carte
        public struct carte
        {
            public char valeur;
            public int famille;
        };

        // Liste des combinaisons possibles
        public enum combinaison { RIEN, PAIRE, DOUBLE_PAIRE, BRELAN, QUINTE, FULL, COULEUR, CARRE, QUINTE_FLUSH };

        // Valeurs des cartes : As, Roi,...
        public static char[] valeurs = { 'A', 'R', 'D', 'V', 'X', '9', '8', '7', '6', '5', '4', '3', '2' };

        // Codes ASCII (3 : coeur, 4 : carreau, 5 : trèfle, 6 : pique)
        public static char[] familles = { '\u2665', '\u2666', '\u2663', '\u2660' };

        // Numéros des cartes à échanger
        public static int[] echange = { 0, 0, 0, 0 };

        // Jeu de 5 cartes
        public static carte[] MonJeu = new carte[5];

        //----------
        // FONCTIONS
        //----------

        // Génère aléatoirement une carte : {valeur;famille}
        // Retourne une expression de type "structure carte"
        public static carte tirage()
        {
            carte tirer;    // initialise la carte
            Random rnd = new Random();  // initialise un random
            int v = rnd.Next(0, 12);    // crée un entier correspondant à v et f pour faire un random de chiffres
            int f = rnd.Next(0, 3);
            tirer.valeur = valeurs[v];  // récupérer la valeur correspondantes
            tirer.famille = familles[f];
            return tirer;   
        }

        // Indique si une carte est déjà présente dans le jeu
        // Paramètres : une carte, le jeu 5 cartes, le numéro de la carte dans le jeu
        // Retourne un entier (booléen)
        public static bool carteUnique(carte uneCarte, carte[] unJeu, int numero)
        {
            for (int m = 0; m < 5; m++)
            {
                if (m == numero) continue;      // compare le jeu m au jeu numero // continue : si la condition est vrai alors retour faux ( passe if )
                {
                    if (uneCarte.valeur == unJeu[m].valeur && uneCarte.famille == unJeu[m].famille)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        // Calcule et retourne la COMBINAISON (paire, double-paire... , quinte-flush)
        // pour un jeu complet de 5 cartes.
        // La valeur retournée est un élement de l'énumération 'combinaison' (=constante)
        public static combinaison cherche_combinaison(ref carte[] unJeu)
        {
            combinaison combi = new combinaison();
            combi = combinaison.RIEN;                   // initialisation de la combinaison à rien
            int[] similaire = { 0, 0, 0, 0, 0 };
            int mfam = 0;                               // initialisation de la verification de cartes de même famille
            int doublepaire = 0;                        // initialisiation d'un compteur pour une dp
            char[,] quintes = { { 'X', 'V', 'D', 'R', 'A' }, { '9', 'X', 'V', 'D', 'R' }, { '8', '9', 'X', 'V', 'D' }, { '7', '8', '9', 'X', 'V' } }; // implémentation de possibilités de quintes
            bool full = false;                           // initialisation booléen permettant de vérifier un full
            bool bre = false;                           // initialisation booléen permettant de vérifier un brelan
            bool qui = false;                           // initialisation booléen permettant de vérifier une quinte
            int z = similaire.Sum();                    // z intègre la somme du tableau similaire
            int memoire = 0;
            int j;

            for (int i = 0; i<5; i++)           // le temps que 5 n'est pas atteint, on rajoute +1
            {
                for (int a = 0; a < 5; a++)     // le temps que 5 n'est pas atteint, on rajoute +1. Double vérificaton du jeu
                {
                    if (unJeu[i].valeur == unJeu[a].valeur)         // si le jeu i est égale au jeu a 
                    {
                        similaire[i] = similaire[i] + 1;            // on rajoute +1 au compteur
                    }

                    if (unJeu[i].famille == unJeu[a].famille)       // si le jeu i est égale au jeu a
                    {
                        mfam = mfam + 1;                      // on rajoute plus 1 au compteur pour les cartes de même famille
                    }
                }
            }

            for (int b = 0; b < 5; b++)
            {
                if (similaire[b] == 2)
                {
                    combi = combinaison.PAIRE;
                    doublepaire = doublepaire + 1;
                    full = true;
                }
                if (similaire[b] == 3)
                {
                    combi = combinaison.BRELAN;
                    bre = true;
                }
                if (similaire[b] == 4)
                {
                    combi = combinaison.CARRE;
                }
                if (doublepaire / 2 == 2)
                {
                    combi = combinaison.DOUBLE_PAIRE;
                }
            }
                for (int c = 0; c < 5; c++) // Boucle qui parcours la main
                {
                    if (similaire[c] == 1)
                    { 
                        z = z + 1;
                    }
                    if (z == 5)
                    {
                        for (int v = 0; v < 4; v++) // On vérifie l'appartenance aux tableaux instaurés de la quinte
                        {
                            memoire = 0;
                            for (int k = 0; k < 5; k++)
                            {
                                for (j = 0; j < 5; j++)
                                {
                                    if (unJeu[k].valeur == quintes[v, j])
                                    {
                                        memoire = memoire + 1;
                                    }

                                    if (memoire == 5)
                                    {
                                        combi = combinaison.QUINTE;
                                        qui = true;
                                    }
                                }
                            }
                        }
                    }
                }

                if (full == true && bre == true)
                {
                    combi = combinaison.FULL;
                }

                if (qui && mfam == 25) // 25 est le resultat si toutes les cartes sont de la même famille ( 5x5 )
                {
                    combi = combinaison.QUINTE_FLUSH;
                }

                if (!qui && mfam == 25)
                {
                    combi = combinaison.COULEUR;
                }
            
            return combi;
        }
        

        // Echange des cartes
        // Paramètres : le tableau de 5 cartes et le tableau des numéros des cartes à échanger
        private static void echangeCarte(ref carte[] unJeu, ref int[] e) // e = tableau qui contient les changement voulus par l'utilisateur
        {
            for (int i = 0; i < e.Length; i++)
            {
                int a = e[i];   // on incrémente dans a le résultat de la demande de l'échange dans le jeu i
                do
                {
                    unJeu[a] = tirage();        // appelle tirage pour l'inclure dans le jeu
                } 
                while (!carteUnique(unJeu[e[i]], unJeu, a));    // tant que la carte n'est pas unique alors, on pioche
            }
        }

        // Tirage d'un jeu de 5 cartes
        // Paramètre : le tableau de 5 cartes à remplir
        private static void tirageDuJeu(ref carte[] unJeu)
        {
            for (int i = 0; i < 5; i++) // le temps que 5 n'est pas atteint +1
            {
                do
                {
                    unJeu[i] = tirage(); // appelle tirage pour l'inclure dans un jeu
                }
                while (!carteUnique(unJeu[i], unJeu, i)); // tant que la carte n'est pas unique alors, on pioche

            }
        }
        // Affiche à l'écran une carte {valeur;famille} en fournisant la colonne de départ
        private static void affichageCarte(ref carte uneCarte)
        {
            //----------------------------
            // TIRAGE D'UN JEU DE 5 CARTES
            //----------------------------
            int left = 0;
            int c = 1;
            // Tirage aléatoire de 5 cartes
            for (int i = 0; i < 5; i++)
            {
                // Tirage de la carte n°i (le jeu doit être sans doublons !)

                // Affichage de la carte
                
                if (MonJeu[i].famille == '\u2665' || MonJeu[i].famille == '\u2666')
                    SetConsoleTextAttribute(hConsole, 252);
                else
                    SetConsoleTextAttribute(hConsole, 240);
                Console.SetCursorPosition(left, 5);
                Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '*', '-', '-', '-', '-', '-', '-', '-', '-', '-', '*');
                Console.SetCursorPosition(left, 6);
                Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', (char)MonJeu[i].famille, ' ', (char)MonJeu[i].famille, ' ', (char)MonJeu[i].famille, ' ', (char)MonJeu[i].famille, ' ', (char)MonJeu[i].famille, '|');
                Console.SetCursorPosition(left, 7);
                Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|');
                Console.SetCursorPosition(left, 8);
                Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', (char)MonJeu[i].famille, ' ', ' ', ' ', ' ', ' ', ' ', ' ', (char)MonJeu[i].famille, '|');
                Console.SetCursorPosition(left, 9);
                Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', ' ', ' ', ' ', (char)MonJeu[i].valeur, (char)MonJeu[i].valeur, (char)MonJeu[i].valeur, ' ', ' ', ' ', '|');
                Console.SetCursorPosition(left, 10);
                Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', (char)MonJeu[i].famille, ' ', ' ', (char)MonJeu[i].valeur, (char)MonJeu[i].valeur, (char)MonJeu[i].valeur, ' ', ' ', (char)MonJeu[i].famille, '|');
                Console.SetCursorPosition(left, 11);
                Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', ' ', ' ', ' ', (char)MonJeu[i].valeur, (char)MonJeu[i].valeur, (char)MonJeu[i].valeur, ' ', ' ', ' ', '|');
                Console.SetCursorPosition(left, 12);
                Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', (char)MonJeu[i].famille, ' ', ' ', ' ', ' ', ' ', ' ', ' ', (char)MonJeu[i].famille, '|');
                Console.SetCursorPosition(left, 13);
                Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|');
                Console.SetCursorPosition(left, 14);
                Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', (char)MonJeu[i].famille, ' ', (char)MonJeu[i].famille, ' ', (char)MonJeu[i].famille, ' ', (char)MonJeu[i].famille, ' ', (char)MonJeu[i].famille, '|');
                Console.SetCursorPosition(left, 15);
                Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '*', '-', '-', '-', '-', '-', '-', '-', '-', '-', '*');
                Console.SetCursorPosition(left, 16);
                SetConsoleTextAttribute(hConsole, 10);
                Console.Write("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", ' ', ' ', ' ', ' ', ' ', c, ' ', ' ', ' ', ' ', ' ');
                left = left + 15;
                c++;
            }

        }

//        public static string Chiffrement(string text)           // chiffrement césar par code Ascii
//        {
//          string fin = "";                        // initialise fin en chaine de charactere vide

//          for (int i = 0; i < text.Length; i++)
//            {
//                fin += (char)(text[i] + 3);     // se décale de 3 valeurs pour chiffrer le code
//            }
//            return fin;
//        }

//        public static string Dechiffrement(string text)
//        {
//            string fin = "";

//           for (int i = 0; i < text.Length; i++)
//            {
//               fin += (char)(text[i] - 3);
//            }
//           return fin;
//        }


        //--------------------
        // Fonction PRINCIPALE
        //--------------------
        static void Main(string[] args)
        {
            //---------------
            // BOUCLE DU JEU
            //---------------
            string reponse;

            Console.OutputEncoding = Encoding.GetEncoding(65001);

            SetConsoleTextAttribute(hConsole, 012);
            while (true)
            {
                // Positionnement et affichage
                Console.Clear();
                Console.WriteLine("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '*', '-', '-', '-', '-', '-', '-', '-', '-', '-', '*');
                Console.WriteLine("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', ' ', ' ', 'P', 'O', 'K', 'E', 'R', ' ', ' ', '|');
                Console.WriteLine("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', ' ', '|');
                Console.WriteLine("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', ' ', '1', ' ', 'J', 'o', 'u', 'e', 'r', ' ', '|');
                Console.WriteLine("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', ' ', '2', ' ', 'S', 'c', 'o', 'r', 'e', ' ', '|');
                Console.WriteLine("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '|', ' ', '3', ' ', 'F', 'i', 'n', ' ', ' ', ' ', '|');
                Console.WriteLine("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}{10}\n", '*', '-', '-', '-', '-', '-', '-', '-', '-', '-', '*');
                Console.WriteLine();
                // Lecture du choix


                do
                {
                    SetConsoleTextAttribute(hConsole, 014);
                    Console.Write("Votre choix : ");
                    reponse = Console.ReadLine();
                }
                while (reponse != "1" && reponse != "2" && reponse != "3");
                Console.Clear();
                SetConsoleTextAttribute(hConsole, 015);
                // Jouer au Poker
                if (reponse == "1")
                {
                    int i = 0;
                    tirageDuJeu(ref MonJeu);
                    affichageCarte(ref MonJeu[i]);

                    // Nombre de carte à échanger
                    try
                    {
                        int compteur = 0;
                        SetConsoleTextAttribute(hConsole, 012);
                        Console.Write("Nombre de cartes a echanger <0-5> ? : ");
                        compteur = int.Parse(Console.ReadLine());
                        int[] e = new int[compteur];
                        for (int j = 0; j < e.Length; j++)
                        {
                            Console.Write("Carte <1-5> : ");

                            e[j] = int.Parse(Console.ReadLine());
                            e[j] -= 1;
                        }

                        echangeCarte(ref MonJeu, ref e);

                    }
                    catch { }
                    //---------------------------------------
                    // CALCUL ET AFFICHAGE DU RESULTAT DU JEU
                    //---------------------------------------
                    Console.Clear();
                    affichageCarte(ref MonJeu[i]);
                    SetConsoleTextAttribute(hConsole, 012);
                    Console.Write("RESULTAT - Vous avez : ");
                    try
                    {
                        // Test de la combinaison
                        switch (cherche_combinaison(ref MonJeu))
                        {
                            case combinaison.RIEN:
                                Console.WriteLine("rien du tout... desole!"); break;
                            case combinaison.PAIRE:
                                Console.WriteLine("une simple paire..."); break;
                            case combinaison.DOUBLE_PAIRE:
                                Console.WriteLine("une double paire; on peut esperer..."); break;
                            case combinaison.BRELAN:
                                Console.WriteLine("un brelan; pas mal..."); break;
                            case combinaison.QUINTE:
                                Console.WriteLine("une quinte; bien!"); break;
                            case combinaison.FULL:
                                Console.WriteLine("un full; ouahh!"); break;
                            case combinaison.COULEUR:
                                Console.WriteLine("une couleur; bravo!"); break;
                            case combinaison.CARRE:
                                Console.WriteLine("un carre; champion!"); break;
                            case combinaison.QUINTE_FLUSH:
                                Console.WriteLine("une quinte-flush; royal!"); break;
                        };
                    }
                    catch { }
                    Console.ReadKey();
                    char enregister = ' ';
                    string nom = "";                // Attends une réponse, relier au console write
                    BinaryWriter f;                 // Stocker et récupère les paramètres
                    SetConsoleTextAttribute(hConsole, 014);
                    Console.Write("Enregistrer le Jeu ? (O/N) : ");
                    enregister = char.Parse(Console.ReadLine());
                    enregister = Char.ToUpper(enregister);

                    if (enregister == 'O')
                    {
                        // const string fileName = "scores.txt";
                        Console.WriteLine("Vous pouvez saisir votre nom (ou pseudo) : ");
                        nom = Console.ReadLine();
                        

                        using (f = new BinaryWriter(new FileStream("scores.txt", FileMode.Create, FileAccess.Write)))
                        {
                            f.Write(nom);
                            for (int a = 0; a < 5; a++) 
                            {
                                f.Write(MonJeu[a].valeur);
                                f.Write(MonJeu[a].famille);
                            }
                        }
                        Console.WriteLine("Score enregistre!");
                        Console.ReadKey();
                    }

                }

               if(reponse == "2")
                {
                    string articles;
                    char[] délimiteurs = { ';' };
                    carte UneCarte;
                    string nom;
                    char c;
                    Array c1;
                    if (File.Exists("scores.txt"))
                    {
                        using (BinaryReader f = new BinaryReader(new FileStream("scores.txt", FileMode.Open, FileAccess.Read)))
                        {
                            nom = f.ReadString();
                            for (int s = 0; s < 5; s++)
                            {
                                MonJeu[s].valeur = f.ReadChar();

                                c = f.ReadChar();

                                if (char.ToString(c) == "")
                                {
                                    MonJeu[s].famille = '\u2665';
                                }
                                if (char.ToString(c) == "")
                                {
                                    MonJeu[s].famille = '\u2666';
                                }
                                if (char.ToString(c) == "")
                                {
                                    MonJeu[s].famille = '\u2663';
                                }
                                if (char.ToString(c) == "")
                                {
                                    MonJeu[s].famille = '\u2660';
                                }

                                c1 = f.ReadChars(3);
                            }
                        }
                        Console.WriteLine("Nom : " + nom);
                        affichageCarte(ref MonJeu[0]);
                        Console.ReadKey();
                    }
                }

                if (reponse == "3")
                    break;

            }
            Console.Clear();
            Console.ReadKey();
        }
    }
}
