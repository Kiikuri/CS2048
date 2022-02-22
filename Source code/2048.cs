using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;
using System;
using System.Collections.Generic;

public class _2048 : PhysicsGame
{
    /// Palikan määritys, ja taustakuvan lataaminen.
    Image taustaKuva = LoadImage("taulu");
    Image voitto = LoadImage("voitto");
    PhysicsObject palikka;

    public override void Begin()
    {
        Restart();
    }


    /// Pelin uudelleen käynnistys.
    public void Restart()
    {
        /// Pelin käynnistys. Antaa taustakuvan, Luo kentän, Asettaa ohjaimet, ja Luo pelin matriisin, johon kaikki pohjautuu.
        ClearAll();
        Camera.ZoomToLevel();
        Level.Background.Image = taustaKuva;
        Level.CreateBorders();
        /// Matriisin luonti.
        int[,] t = {
           {0,0,0,0},
           {0,0,0,0},
           {0,0,0,0},
           {0,0,0,0}
        };
        LuoKentta(t);
        AsetaOhjaimet(t);
    }


    /// Luo kentän, tarkistaa matriisin luvun 0 varalta, ja lisää satunnaisesti joko luvun 2 tai 4 matriisiin.
    /// Tarkistaa myös koko matriisin muiden lukujen varalta, ja lähettää komennot luoda palikat.
    public void LuoKentta(int[,] t)
    {
        ClearGameObjects();
        TarkistaNolla(t);
        int j = 0;
        int k = 0;
        while (j < 4)
        {
            k = 0;
            /// Palikoiden luontikutsut. Aliohjelma katsoo koko matriisin läpi ja vertaa siinä olevia lukuja palikoiden arvoihin. 
            /// Mikäli jossain kohtaa matriisia on esim luku 2. Siihen kohtaan luodaan palikka, jonka arvo on 2.
            while (k < 4)
            {
                if (t[j, k] != 0) LuoPalikka(j, k, t[j, k].ToString());
                if (t[j, k] == 2048)
                {
                    LuoPalikka(j, k, t[j, k].ToString());
                    ClearGameObjects();
                    Level.Background.Image = voitto;
                    return;
                }
                k++;
            }
            j++;
        }
    }


    /// Ohjainten määrittely.
    public void AsetaOhjaimet(int[,] t)
    {
        /// WASD liikutus.
        Keyboard.Listen(Key.H, ButtonState.Pressed, ShowControlHelp, "Näytä ohjeet");
        Keyboard.Listen(Key.W, ButtonState.Pressed, delegate { TarkistaYlos(t); }, "Liikuta ylös");
        Keyboard.Listen(Key.S, ButtonState.Pressed, delegate { TarkistaAlas(t); }, "Liikuta alas");
        Keyboard.Listen(Key.A, ButtonState.Pressed, delegate { TarkistaVasen(t); }, "Liikuta vasemmalle");
        Keyboard.Listen(Key.D, ButtonState.Pressed, delegate { TarkistaOikea(t); }, "Liikuta oikealle");

        /// Nuolinäppäin liikutus.
        Keyboard.Listen(Key.Up, ButtonState.Pressed, delegate { TarkistaYlos(t); }, "Liikuta ylös");
        Keyboard.Listen(Key.Down, ButtonState.Pressed, delegate { TarkistaAlas(t); }, "Liikuta alas");
        Keyboard.Listen(Key.Left, ButtonState.Pressed, delegate { TarkistaVasen(t); }, "Liikuta vasemmalle");
        Keyboard.Listen(Key.Right, ButtonState.Pressed, delegate { TarkistaOikea(t); }, "Liikuta oikealle");

        /// Muut kontrollit.
        Keyboard.Listen(Key.R, ButtonState.Pressed, Restart, "Aloita Alusta");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }


    /// Aliohjelma, joka tarkistaa luvun 0 varalta, ja luo satunnaisesti luvun 2 tai 4 ja lisää sen matriisiin.
    public void TarkistaNolla(int[,] t)
    {
        int n = 1;
        int summa = 0;
        int z = 0;
        int v = 0;
        while (z <= 3)
        {
            v = 0;
            while (v <= 3)
            {
                if (t[v, z] == 0)
                {
                    summa += 1;
                    break;
                }
                v++;
            }
            if (summa > 0) break;
            z++;
        }
        /// Jos summa ylittää luvun 0, niin Aliohjelma luo satunnaisesti luvun 2 tai 4 ja lisää sen matriisiin satunnaiseen vapaaseen kohtaan.
        if (summa > 0)
        {
            while (n > 0)
            {
                /// Satunnainen luku 2 tai 4.
                Random random = new Random();
                int k = random.Next(1, 101);
                if (k >= 90) k = 4;
                else k = 2;

                /// Satunnainen X koordinaatti matriisiin.
                Random aks = new Random();
                int j = random.Next(0, 4);

                /// Satunnainen y koordinaatti matriisiin.
                Random yy = new Random();
                int h = random.Next(0, 4);

                /// Ohjelma kokeilee niin montaa koordinaattia, kunnes se löytää satunnaisesti vapaan paikan matriisista.
                if (t[h, j] == 0)
                {
                    t[h, j] = k;
                    break;
                }
            }
        }
    }


    /// Vasemmalle liikkuminen.
    private void TarkistaVasen(int[,] t)
    {
        int summa = 0;
        int x = 0;
        int y = 0;
        int a = 0;
        ///siirtää numerot matriisissa vasemmalle, mikäli välissä on jossain kohtaa luku 0.
        while (a < 3)
        {
            while (y <= 3)
            {
                x = 0;
                while (x <= 2)
                {
                    if (t[y, x] == 0)
                    {
                        t[y, x] = t[y, x + 1];
                        /// Tarkistaa, liikkuiko palikat.
                        if (t[y, x] == t[y, x + 1])
                        {
                            summa++;
                            if (t[y, x] == 0) summa--;
                        }
                        t[y, x + 1] = 0;
                    }
                    x++;
                }
                y++;
            }
            a++;
            y = 0;
        }
        ///yhdistää luvut, mikäli jonkin luvun vieressä on toinen saman arvoinen.
        while (y <= 3)
        {
            x = 0;
            while (x <= 2)
            {
                if (t[y, x] == t[y, x + 1])
                {
                    /// Tarkistaa, liikkuiko palikat.
                    if (t[y, x] == t[y, x + 1])
                    {
                        summa++;
                        if (t[y, x] == 0) summa--;
                    }
                    t[y, x] = t[y, x] + t[y, x + 1];
                    if (x + 2 <= 3)
                    {
                        t[y, x + 1] = t[y, x + 2];
                        if (x + 3 <= 3)
                        {
                            t[y, x + 2] = t[y, x + 3];
                        }
                    }
                    t[y, 3] = 0;
                }
                x++;
            }
            y++;
        }
        /// Luo kentän uudelleen, mikäli palikat liikkuvat.
        if (summa > 0) LuoKentta(t);
        summa = 0;
    }


    /// Oikealle liikkuminen.
    private void TarkistaOikea(int[,] t)
    {
        int summa = 0;
        int x = 0;
        int y = 3;
        int a = 0;
        ///siirtää numerot matriisissa oikealle, mikäli välissä on jossain kohtaa luku 0.
        while (a < 4)
        {
            while (y <= 3)
            {
                x = 3;
                while (x > 0)
                {
                    if (t[y, x] == 0)
                    {
                        t[y, x] = t[y, x - 1];
                        /// Tarkistaa, liikkuiko palikat.
                        if (t[y, x] == t[y, x - 1])
                        {
                            summa++;
                            if (t[y, x] == 0) summa--;
                        }
                        t[y, x - 1] = 0;
                    }
                    x--;
                }
                y++;
            }
            a++;
            y = 0;
        }
        ///yhdistää luvut, mikäli jonkin luvun vieressä on toinen saman arvoinen.
        y = 0;
        x = 3;
        while (y <= 3)
        {
            x = 3;
            while (x >= 0)
            {
                if (x - 1 >= 0)
                {
                    if (t[y, x] == t[y, x - 1])
                    {
                        /// Tarkistaa, liikkuiko palikat.
                        if (t[y, x] == t[y, x - 1])
                        {
                            summa++;
                            if (t[y, x] == 0) summa--;
                        }
                        t[y, x] = t[y, x] + t[y, x - 1];
                        if (x - 2 >= 0)
                        {
                            t[y, x - 1] = t[y, x - 2];
                            if (x - 3 >= 0)
                            {
                                t[y, x - 2] = t[y, x - 3];
                            }
                        }
                        t[y, 0] = 0;
                    }
                }
                x--;
            }
            y++;
        }
        /// Luo kentän uudelleen, mikäli palikat liikkuvat. 
        if (summa > 0) LuoKentta(t);
        summa = 0;
    }


    /// ylös liikkuminen.
    private void TarkistaYlos(int[,] t)
    {
        int summa = 0;
        int x = 0;
        int y = 0;
        int a = 0;
        ///siirtää numerot matriisissa ylös, mikäli välissä on jossain kohtaa luku 0.
        while (a < 4)
        {
            while (x <= 3)
            {
                y = 0;
                while (y < 3)
                {
                    if (t[y, x] == 0)
                    {
                        t[y, x] = t[y + 1, x];
                        /// Tarkistaa, liikkuiko palikat.
                        if (t[y, x] == t[y + 1, x])
                        {
                            summa++;
                            if (t[y, x] == 0) summa--;
                        }
                        t[y + 1, x] = 0;
                    }
                    y++;
                }
                x++;
            }
            a++;
            x = 0;
        }
        ///yhdistää luvut, mikäli jonkin luvun vieressä on toinen saman arvoinen.
        y = 0;
        x = 0;
        while (x <= 3)
        {
            y = 0;
            while (y <= 3)
            {
                if (y + 1 <= 3)
                {
                    if (t[y, x] == t[y + 1, x])
                    {
                        /// Tarkistaa, liikkuiko palikat.
                        if (t[y, x] == t[y + 1, x])
                        {
                            summa++;
                            if (t[y, x] == 0) summa--;
                        }
                        t[y, x] = t[y, x] + t[y + 1, x];
                        if (y + 2 <= 3)
                        {
                            t[y + 1, x] = t[y + 2, x];
                            if (y + 3 <= 3)
                            {
                                t[y + 2, x] = t[y + 3, x];
                            }
                        }
                        t[3, x] = 0;
                    }
                }
                y++;
            }
            x = x + 1;
        }
        /// Luo kentän uudelleen, mikäli palikat liikkuvat.
        if (summa > 0) LuoKentta(t);
        summa = 0;
    }


    /// Alas liikkuminen.
    private void TarkistaAlas(int[,] t)
    {
        int summa = 0;
        int x = 0;
        int y = 3;
        int a = 0;
        ///siirtää numerot matriisissa alas, mikäli välissä on jossain kohtaa luku 0.
        while (a < 4)
        {
            while (x <= 3)
            {
                y = 3;
                while (y > 0)
                {
                    if (t[y, x] == 0)
                    {
                        t[y, x] = t[y - 1, x];
                        /// Tarkistaa, liikkuiko palikat.
                        if (t[y, x] == t[y - 1, x])
                        {
                            summa++;
                            if (t[y, x] == 0) summa--;
                        }
                        t[y - 1, x] = 0;
                    }
                    y--;
                }
                x++;
            }
            a++;
            x = 0;
        }
        ///yhdistää luvut, mikäli jonkin luvun vieressä on toinen saman arvoinen.
        y = 3;
        x = 0;
        while (x <= 3)
        {
            y = 3;
            while (y >= 0)
            {
                if (y - 1 >= 0)
                {
                    if (t[y, x] == t[y - 1, x])
                    {
                        /// Tarkistaa, liikkuiko palikat.
                        if (t[y, x] == t[y - 1, x])
                        {
                            summa++;
                            if (t[y, x] == 0) summa--;
                        }
                        t[y, x] = t[y, x] + t[y - 1, x];
                        if (y - 2 >= 0)
                        {
                            t[y - 1, x] = t[y - 2, x];
                            if (y - 3 >= 0)
                            {
                                t[y - 2, x] = t[y - 3, x];
                            }
                        }
                        t[0, x] = 0;
                    }
                }
                y--;
            }
            x++;
        }
        /// Luo kentän uudelleen, mikäli palikat liikkuvat.
        if (summa > 0) LuoKentta(t);
        summa = 0;
    }


    /// Pelin palikoiden luominen. Nämä seuraavat 11 aliohjelmaa ovat lähes täysin identtisiä. Ainoat erot ovat tekstuurit ja aliohjelman nimi.
    public void LuoPalikka(int a, int b, string numero)
    {
        /// Lataa tekstuurin palikalle.
        Image palikkaKuva = LoadImage(numero);
        /// Alku X ja Y koordinaatit matriisin kohdassa (0, 0).
        double x = -292.5;
        double y = 292.5;
        /// Ero X ja Y koordinaateissa, joka lisätään alku koordinaatteihin, jotta saadaan palikka halutulle alueelle. arvot "a" ja "b" saavat arvon 0 ja 3 väliltä, joka määrittää palikan rivin.
        double xEro = b * 195;
        double yEro = a * 195;
        /// Palikan luonti.
        palikka = new PhysicsObject(175, 175);
        palikka.X = x + xEro;
        palikka.Y = y - yEro;
        palikka.Image = palikkaKuva;
        Add(palikka);
    }
}

