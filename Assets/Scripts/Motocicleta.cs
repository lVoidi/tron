using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public class Motocicleta
    {
        public Componente Cabeza;
        public ColaItems items;
        public int tamagnoEstela = 4;
        public int velocidad = 5;
        public int combustible = 100;
        public bool esInmune = false;

        public Motocicleta()
        {
            Cabeza = new Componente();
            Cabeza.nodo = new Nodo();
            Cabeza.nodo.x = 0;
            Cabeza.nodo.y = 0;
        }

        public void UtilizarItem()
        {
            Item item = items.SacarItem();
            if (item == null)
            {
                return;
            }
            if (item.nombre == "Combustible")
            {
                combustible += 10;
            }
            if (item.nombre == "Estela")
            {
                esInmune = true;
            }
        }

        public class Componente
        {
            public Nodo nodo;
            public Componente siguiente;
        }

        public class PilaPoderes
        {
            public Poder[] poderes;
            public int cantidadPoderes = 0;
            public int maxPoderes = 10;

            public PilaPoderes()
            {
                poderes = new Poder[maxPoderes];
            }

            public void AgregarPoder(Poder poder)
            {
                if (cantidadPoderes < maxPoderes)
                {
                    poderes[cantidadPoderes] = poder;
                    cantidadPoderes++;
                }
            }

            public Poder SacarPoder()
            {
                if (cantidadPoderes > 0)
                {
                    Poder poder = poderes[cantidadPoderes - 1];
                    cantidadPoderes--;
                    return poder;
                }
                return null;
            }
        }

        public class Poder
        {
            public string nombre;
        }

        public class ColaItems
        {
            public Item[] items;
            public int cantidadItems = 0;
            public int maxItems = 10;

            public ColaItems()
            {
                items = new Item[maxItems];
            }

            public void AgregarItem(Item item)
            {
                if (cantidadItems < maxItems)
                {
                    items[cantidadItems] = item;
                    cantidadItems++;
                }
            }

            public Item SacarItem()
            {
                if (cantidadItems > 0)
                {
                    Item item = items[0];
                    for (int i = 0; i < cantidadItems - 1; i++)
                    {
                        items[i] = items[i + 1];
                    }
                    cantidadItems--;
                    return item;
                }
                return null;
            }
        }
        public class Item
        {
            public string nombre;
        }
    }
}

