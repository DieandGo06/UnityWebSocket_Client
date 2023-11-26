using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


/* "FuncionTimer" y "AdministradorTareas" funciona muy similar.
 * 
 * FuncionTimer: crea un objeto al que se le paso una funcion que ejecutara pasado cierto tiempo y luego destruirse
 * AdministradorTareas: hace lo mismo pero guardando las acciones en una lista dentro de un solo objeto
 * 
 * A AdministradorTareas le agregue un metodo para que puediera ejecutar un funcion durante "x" tiempo
 * antes de sacarlo de la lista.
 * Si se ejecuta en un update sin condiciones tipo "GetKeyDown", se crea una tarea por cada por frame,
 * imitando a un update normal.
 */



//Este codigo es sacado de: https://www.youtube.com/watch?v=OGyp3jAmpnw&list=PLJ2Wt5mAxvQWQmMED-lc3BcPh5q9RE_p6&index=33&t=5s
public class AdminitradorTareas : MonoBehaviour
{

    void Update()
    {
        //LTS
        foreach (Tareas.Tarea _tarea in Tareas.listaTareas)
        {
            if (Time.time > _tarea.momentoInicio)
            {
                if (_tarea.accion != null)
                {
                    _tarea.accion();
                }
                if (_tarea.accion_string != null)
                {
                    _tarea.accion_string(_tarea.texto);
                }
                if (_tarea.accion_int != null)
                {
                    _tarea.accion_int(_tarea.numeroInt);
                }
                Tareas.listaTareas.Remove(_tarea);
                break;
            }
        }

        //EXPERIMENTAL
        foreach (Tareas.Tarea _tarea in Tareas.listaTareasConDuracion)
        {
            if (Time.time > _tarea.momentoInicio)
            {
                if (_tarea.accion != null)
                {
                    _tarea.accion();
                }
            }
            if (Time.time > _tarea.momentoInicio + _tarea.duracion)
            {
                Tareas.listaTareasConDuracion.Remove(_tarea);
                
                break;
            }

        }
    }


}


public static class Tareas
{
    public class Tarea
    {
        public float momentoInicio;
        public Action accion;

        public Action<string> accion_string;
        public Action<int> accion_int;
        public string texto;
        public int numeroInt;

        //Experimentales
        public int id;
        public float duracion;
    }
    //LTS
    public static List<Tarea> listaTareas = new List<Tarea>();

    //Experimental
    static List<int> listaID = new List<int>();
    public static List<Tarea> listaTareasConDuracion = new List<Tarea>();




    public static void Nueva(float _timer, Action _accion)
    {
        listaTareas.Add(new Tarea
        {
            momentoInicio = Time.time + _timer,
            accion = _accion
        });
    }

    public static void Nueva(float _timer, Action<string> _accion, string _texto)
    {
        listaTareas.Add(new Tarea
        {
            momentoInicio = Time.time + _timer,
            accion_string = _accion,
            texto = _texto
        });
    }

    public static void Nueva(float _timer, Action<int> _accion, int _numero)
    {
        listaTareas.Add(new Tarea
        {
            momentoInicio = Time.time + _timer,
            accion_int = _accion,
            numeroInt = _numero
        });
    }



    //Función experimental 
    public static void NuevaConCooldown(float _timer, float _cooldwon, Action _accion, int _id)
    {
        foreach (int _ID in listaID)
        {
            if (_ID == _id) return;
        }
        listaID.Add(_id);
        Nueva(_cooldwon, () => listaID.Remove(_id));

        listaTareas.Add(new Tarea
        {
            momentoInicio = Time.time + _timer,
            accion = _accion,
            id = _id
        });
    }


    //Función experimental
    public static void NuevaConDuracion(float _timer, float _duracion, float _cooldwon, Action _accion, int _id)
    {
        foreach (int _ID in listaID)
        {
            if (_ID == _id) return;
        }
        listaID.Add(_id);
        Nueva(_cooldwon + _duracion, () => listaID.Remove(_id));

        Tarea nuevaTarea = new Tarea
        {
            momentoInicio = Time.time + _timer,
            duracion = _duracion,
            accion = _accion,
            id = _id
        };
        listaTareasConDuracion.Add(nuevaTarea);


    }
}





