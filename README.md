# Poker 
[![forthebadge](https://forthebadge.com/badges/made-with-c-sharp.svg)](https://forthebadge.com)

Pour vous mettre dans le contexte fictif, la société Hsh et plus particulièrement le service Direction des systèmes d'Informations souhaite
réalisé une maintenance corrective de certains jeux qui se révèle défecteux et présente des réalisations qui n'ont pas de formalités. Nous
étions chargés d'améliorer le jeu qui s'inspire du poker.

Dans ce README, nous expliquerons dans sa globalité le programme final.

En ce qui concerne les règles, nous avons une main de 5 cartes qui seront générés aléatoirement. Une combiniaison peut alors se révéler, 
si ce n'est pas le cas ou que l'on souhaite potentiellement l'améliorer alors on peut choisir un nombre de carte que l'on souhaite
échanger. Après cela, nous obtenons le résultat de cette main, qui peut être nul ou encore faire partie des combinaisons : paire, double
paire, brelan, carré, quinte, full, quinte-flush, et couleur.


## Les Elements techniques

### Tirage

La fonction ```public static carte tirage()``` permet de générer une carte aléatoirement grâce au ```Random()```. Cela crée la structure de la carte avec deux paramètres : valeur et famille.

```c#
public static carte tirage()
  {
    carte tirer;   
    Random rnd = new Random();  
    int v = rnd.Next(0, 12);   
    int f = rnd.Next(0, 3);
    tirer.valeur = valeurs[v];  
    tirer.famille = familles[f];
    return tirer;   
  }
```

### Carte Unique

La fonction ```public static bool carteUnique()``` permet de vérifier qu'il n'est pas de doublon qui se forme dans la main. 
Elle part sur le principe que la carte est unique et que le booléen à retourner sera vrai. 
Par la suite cette fonction permettra de comparer les cartes et retournera faux uniquement si il croise une carte identique.

```c#
public static bool carteUnique(carte uneCarte, carte[] unJeu, int numero)
        {
            for (int m = 0; m < 5; m++)
            {
                if (m == numero) continue;
                {
                    if (uneCarte.valeur == unJeu[m].valeur && uneCarte.famille == unJeu[m].famille)
                    {
                        return false;
                    }
                }
            }
            return true;
        }
```

### Cherche Combinaison

La fonction ```public static combinaison cherche_combinaison()``` permet de regarder si le jeu contient une combinaison.
Elle commence par vérifier le nombre d'arrivée de chaque paramètre ( valeur et famille ) et sont stocker dans des tableaux initialisés dans cette fonction. 
A la suite, il y a les 8 combinaisons possibles qui sont réparties dans leur partie, le ```for``` nous permettant de parcourir à nouveau la main.

```c#
public static combinaison cherche_combinaison(ref carte[] unJeu)
        {
            combinaison combi = new combinaison();
            combi = combinaison.RIEN;                  
            int[] similaire = { 0, 0, 0, 0, 0 };
            int mfam = 0;                              
            int doublepaire = 0;                        
            char[,] quintes = { { 'X', 'V', 'D', 'R', 'A' }, { '9', 'X', 'V', 'D', 'R' }, { '8', '9', 'X', 'V', 'D' }, { '7', '8', '9', 'X', 'V' } };
            int quinte = 0;                             
            bool var = false;                          
            bool bre = false;                           
            bool qui = false;                           
            int z = similaire.Sum();
            int memoire = 0;
            int j;

            for (int i = 0; i<5; i++)           
            {
                for (int a = 0; a < 5; a++)    
                {
                    if (unJeu[i].valeur == unJeu[a].valeur)         
                    {
                        similaire[i] = similaire[i] + 1;            
                    }

                    if (unJeu[i].famille == unJeu[a].famille)       
                    {
                        mfam = mfam + 1;                    
                    }
                }
            }

            for (int b = 0; b < 5; b++)
            {
```

### Les combinaisons

#### Paire 

Cette partie pour la paire permet de regerder si on retrouve un "2" dans notre tableau ```similaire()``` initialisé précédement.
On utilise également un booléen et un compteur ```doublepaire``` qui se mettra à jour à chaque "2" qui se trouvera dans notre tableau.
Quant au booléen, lorsque cette fonction est respecté alors il est égal à true.

```c#
if (similaire[b] == 2)
  {
    combi = combinaison.PAIRE;
    doublepaire = doublepaire + 1;
    full = true;
  }
```

#### Double-Paire

La double-paire utilise le compteur initialisé dans la paire, permettant que lorsqu'elle rencontre deux fois "2" alors elle bascule sur la fontion de la double paire.

```c#
if (doublepaire / 2 == 2)
   {
     combi = combinaison.DOUBLE_PAIRE;
   }
```

#### Brelan

La partie du brelan permet de regerder si on retrouve un "3" dans notre tableau ```similaire()```.

```c#
if (similaire[b] == 3)
  {
    combi = combinaison.BRELAN;
    bre = true;
  }
```

#### Carré

La partie du carré permet de regerder si on retrouve un "4" dans notre tableau ```similaire()```.

```c#
if (similaire[b] == 4)
  {
    combi = combinaison.CARRE;
  }
```

#### Quinte 

La partie de la quinte est séparé en deux parties. La première vérifie que toutes les valeurs sont uniques et stocke le résultat dans notre tableau ```similaire()```. Ensuite, la deuxième partie permet de comparer le jeu aux valeurs stockés dans le tableau ```char[,] quintes``` initialisé au départ de notre cherche combinaison. Si tout ses paramètres sont respectés alors, cela forme une quinte.

```c#
for (int c = 0; c < 5; c++) 
                {
                    if (similaire[c] == 1)
                    { 
                        z = z + 1;
                    }
                    if (z == 5)
                    {
                        for (int v = 0; v < 4; v++) 
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
```

#### Full

La partie du full est un assemblage de nos booléens initialisés lors de la paire et du brelan, si les deux booléens retournent true alors nous avons un full.

```c#
 if (full == true && bre == true)
  {
    combi = combinaison.FULL;
  }
```

#### Quinte Flush

La partie de la quinte flush permet de regarder si toutes nos cartes font parties de la même famille et utilise le booléen initialisé dans la quinte qui retourne true.

```c#
if (qui && mfam == 25)       // 25 est le resultat si toutes les cartes sont de la même famille ( 5x5 )
  {
    combi = combinaison.QUINTE_FLUSH;
  }
```

#### Couleur

La partie de la quinte flush permet de regarder si toutes nos cartes font parties de la même famille et doit être différent de la quinte.

```c#
 if (!qui && mfam == 25)
  {
    combi = combinaison.COULEUR;
  }
```

### Echange Carte

La fonction ```private static void echangeCarte()``` permet de savoir les choix de l'utilisateur concernant l'échange des cartes.
Elle est permise par le tableau ```int[] e``` qui contient les changements voulus par l'utilisateur.
Il fait appel à des fonctions vu précedement : ```tirage()``` et ```carteUnique()```, pour permettre au jeu d'être inclu avec ses modifications.

```c#
 private static void echangeCarte(ref carte[] unJeu, ref int[] e)
        {
            for (int i = 0; i < e.Length; i++)
            {
                int a = e[i];   
                do
                {
                    unJeu[a] = tirage();        
                } 
                while (!carteUnique(unJeu[e[i]], unJeu, a));  
            }
        }
```
### Tirage du jeu

La fonction ```private static void tirageDuJeu()``` permet d'avoir la création de 5 cartes qui constituera la main où toutes les cartes seront uniques.

```c#
 private static void tirageDuJeu(ref carte[] unJeu)
        {
            for (int i = 0; i < 5; i++) 
            {
                do
                {
                    unJeu[i] = tirage();
                }
                while (!carteUnique(unJeu[i], unJeu, i)); 
            }
        }
```

### Affichage Carte
