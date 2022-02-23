using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Laboratorio_No_1.Models
{
    public class Jugador : IComparable
    {
        [Key]
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo Requerido")]
        [Display(Name = "Nombre")]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo Requerido")]
        [Display(Name = "Apellido")]
        public string LastName { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo Requerido")]
        public string Club { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo Requerido")]
        [Display(Name = "Salario")]
        [Range(0,double.MaxValue, ErrorMessage = "Fuera de Rango")]
        public double Salary { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo Requerido")]
        [Display(Name = "Posicion")]
        public string position { get; set; }

        public int CompareTo(Jugador other)
        {
            return other.Id < Id ? -1 : other.Id == Id ? 0 : 1;
        }

        public Comparison<Jugador> CompareById = delegate (Jugador i, Jugador j)
        {
            return i.CompareTo(j);
        };

        public Comparison<Jugador> CompareByName = delegate (Jugador i, Jugador j)
        {
            return i.Name.CompareTo(j.Name);
        };

        public Comparison<Jugador> CompareByLastName = delegate (Jugador i, Jugador j)
        {
            return i.LastName.CompareTo(j.LastName);
        };

        public Comparison<Jugador> CompareByClub = delegate (Jugador i, Jugador j)
        {
            return i.Club.CompareTo(j.Club);
        };

        public Comparison<Jugador> CompareBySalary = delegate (Jugador i, Jugador j)
        {
            return i.Salary.CompareTo(j.Salary);
        };

        public Comparison<Jugador> CompareByPosition = delegate (Jugador i, Jugador j)
        {
            return i.position.CompareTo(j.position);
        };

        public override string ToString()
        {
            return $"{Id}|{Name}|{LastName}|{Club}|{Salary}|{position}";
        }

        public bool Equals(Jugador jugador)
        {
            bool igual = jugador.Name == Name;
            igual = igual && jugador.LastName == LastName;
            igual = igual && jugador.Club == Club;
            igual = igual && jugador.Salary == Salary;
            igual = igual && jugador.position == position;
            return igual;
        }

        public int CompareTo(object obj)
        {
            try
            {
                Jugador jugador = obj as Jugador;
                return jugador.Id < Id ? -1 : jugador.Id == Id ? 0 : 1;
            }
            catch(Exception e)
            {
                throw e;
            }
        }
    }
}