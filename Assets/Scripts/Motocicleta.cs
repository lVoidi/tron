using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts
{
    public class Motocicleta
    {
        public Componente Cabeza;
        public LinkedList<Componente> Estela;
        public ColaItems items;
        public PilaPoderes poderes;
        public Vector2 direccion;
        public int tamagnoEstela = 3;
        public int velocidad = 5;
        public int combustible = 100;
        public bool esInmune = false;
        private Random rng = new Random();

        public Motocicleta(Vector2 dir)
        {
            Cabeza = new Componente();
            Cabeza.nodo = new Nodo();
            Cabeza.nodo.x = 0;
            Cabeza.nodo.y = 0;
            Cabeza.nodo.esCabeza = true;
            direccion = dir;

            Estela = new LinkedList<Componente>();
            for (int i = 0; i < tamagnoEstela; i++)
            {
                Componente componente = new Componente();
                componente.nodo = new Nodo();
                componente.nodo.x = Cabeza.nodo.x;
                componente.nodo.y = Cabeza.nodo.y;
                componente.nodo.esObstaculo = true;
                Estela.AddLast(componente);
            }
        }

        public void UtilizarPoder()
        {
            Poder poder = poderes.SacarPoder();
            if (poder == null)
            {
                return;
            }
            if (poder.nombre == "Inmunidad")
            {
                esInmune = true;
            }
            if (poder.nombre == "Velocidad")
            {
                velocidad += rng.Next(1, 10-velocidad);
            }
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
                combustible += rng.Next(1, 100-combustible);
            }
            else if (item.nombre == "Bomba")
            {
                if (direccion == new Vector2(-1, 0))
                {
                    Cabeza.nodo.Derecha.esObstaculo = true;
                } else if (direccion == new Vector2(1, 0))
                {
                    Cabeza.nodo.Izquierda.esObstaculo = true;
                }
                else if (direccion == new Vector2(0, -1))
                {
                    Cabeza.nodo.Arriba.esObstaculo = true;
                }
                else if (direccion == new Vector2(0, 1))
                {
                    Cabeza.nodo.Abajo.esObstaculo = true;
                }
            }
            else if (item.nombre == "Estela")
            {
                tamagnoEstela++;
            }
        }
    }
    public class Componente
    {
        public Nodo nodo;
    }

    public class PilaPoderes
    {
        public LinkedList<Poder> poderes;

        public PilaPoderes()
        {
            poderes = new LinkedList<Poder>();
        }

        public void AgregarPoder(Poder poder)
        {
            poderes.AddFirst(poder);
        }

        public Poder SacarPoder()
        {
            if (poderes.Count > 0)
            {
                Poder poder = poderes.First.Value;
                poderes.RemoveFirst();
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


