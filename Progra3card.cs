using System;
using MySql.Data.MySqlClient;
using MySqlConnection = MySql.Data.MySqlClient.MySqlConnection;
using MySqlCommand = MySql.Data.MySqlClient.MySqlCommand;
using MySqlDataReader = MySql.Data.MySqlClient.MySqlDataReader;

namespace Progra3Card.Administrativo
{
    class Program
    {
        private static string connectionString = "Server=localhost;Database=mi_banco_db;Uid=root;Pwd=root;";

        static void Main(string[] args)
        {
            bool salir = false;
            while (!salir)
            {
                Console.Clear();
                Console.WriteLine("========================================");
                Console.WriteLine("    SISTEMA ADMINISTRATIVO PROGRA3CARD   ");
                Console.WriteLine("========================================");
                Console.WriteLine("1. Emitir Nueva Tarjeta (Alta de Cliente)");
                Console.WriteLine("2. Listar Tarjetas");
                Console.WriteLine("3. Ver Detalle de una Tarjeta / Cliente");
                Console.WriteLine("4. Eliminar Tarjeta (Baja de Sistema)");
                Console.WriteLine("5. Emitir Nueva Liquidación Mensual");
                Console.WriteLine("6. Salir");
                Console.WriteLine("========================================");
                Console.Write("Seleccione una opción: ");

                switch (Console.ReadLine())
                {
                    case "1": MenuEmitirTarjeta(); break;
                    case "2": MenuListarTarjetas(); break;
                    case "3": MenuVerDetalleTarjeta(); break;
                    case "4": MenuEliminarTarjeta(); break;
                    case "5": MenuEmitirLiquidacion(); break;
                    case "6": salir = true; break;
                    default:
                        Console.WriteLine("Opción no válida. Presione una tecla para continuar...");
                        Console.ReadKey();
                        break;
                }
            }
        }

        // Funciones a completar:
        static void MenuEmitirTarjeta()
        {
            altaClienteyTarjeta();
        }
        static void MenuListarTarjetas()
        {
            Console.Clear();
            Console.WriteLine("--- LISTADO GENERAL DE TARJETAS ---");
            Console.WriteLine("{0,-12} {1,-18} {2,-20} {3,-15}", "Nro Cuenta", "Nro Tarjeta", "Banco Emisor", "DNI Titular");
            Console.WriteLine("----------------------------------------------------------------------");

            // === A realizar ===
            // Aquí deben implementar un SELECT sobre la tabla 'tarjetas'
            // para recorrer las filas e imprimirlas en la consola.
            
            ObtenerYMostrarTarjetas();

            Console.WriteLine("\nPresione una tecla para volver al menú...");
            Console.ReadKey();
        }

        static void MenuVerDetalleTarjeta()
        {
            Console.Clear();
            Console.WriteLine("--- DETALLE DE TARJETA Y CLIENTE ---");
            Console.Write("Ingrese el Número de Cuenta a consultar: ");
            int numCuenta = Convert.ToInt32(Console.ReadLine());

            // === A realizar ===
            // Aquí deben realizar un SELECT con un JOIN entre 'tarjetas' y 'usuarios' 
            // filtrando por el numCuenta para traer todos los campos (Nombre, Apellido, Email, Saldo, etc.)
            
            MostrarDetalleCompleto(numCuenta);

            Console.WriteLine("\nPresione una tecla para volver al menú...");
            Console.ReadKey();
        }

        static void MenuEliminarTarjeta()
        {
            Console.Clear();
            Console.WriteLine("--- ELIMINAR TARJETA DEL SISTEMA ---");
            Console.Write("Ingrese el Número de Cuenta de la tarjeta a dar de baja: ");
            int numCuenta = Convert.ToInt32(Console.ReadLine());

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("\n⚠️ ADVERTENCIA: Se eliminará la tarjeta, sus liquidaciones y los datos de acceso web vinculados.");
            Console.ResetColor();
            Console.Write("¿Está seguro de continuar? (S/N): ");
            
            if (Console.ReadLine().ToUpper() == "S")
            {
                // === A realizar ===
                // Aquí deben ejecutar un DELETE sobre la tabla 'tarjetas' donde num_cuenta = numCuenta.
                // Como definimos ON DELETE CASCADE en la base de datos, las liquidaciones se borrarán solas.
                // Opcional: Evaluar si también eliminan al usuario de la tabla 'usuarios' o si lo mantienen.
                
                bool exito = DarDeBajaTarjeta(numCuenta);

                if (exito)
                    Console.WriteLine("\nTarjeta eliminada correctamente del sistema.");
                else
                    Console.WriteLine("\nError al intentar eliminar la tarjeta. Verifique el número de cuenta.");
            }
            else
            {
                Console.WriteLine("\nOperación cancelada.");
            }

            Console.WriteLine("\nPresione una tecla para volver al menú...");
            Console.ReadKey();
        }

        static void MenuEmitirLiquidacion()
        {
            Console.Clear();
            Console.WriteLine("=== EMITIR LIQUIDACION ===");
            Console.Write("Ingrese el Número de Cuenta a la cual se le emitira una liquidacion nueva: ");
            int numCuenta = Convert.ToInt32(Console.ReadLine());

            using(MySqlConnection conexion = new MySqlConnection(connectionString))
            {
                try
                {
                    conexion.Open();

                    string query = "SELECT estado FROM tarjetas WHERE num_cuenta = @numCuenta";
                    string estado ="";
                    bool cliente_encontrado = false;

                    using(MySqlCommand comando = new MySqlCommand(query,conexion))
                    {
                        comando.Parameters.AddWithValue("@numCuenta",numCuenta);

                        using(MySqlDataReader lector = comando.ExecuteReader())
                        {
                            if(lector.Read())
                            {
                                cliente_encontrado = true;
                                estado = lector["estado"].ToString()??"";
                            }
                        }
                    }

                    if(cliente_encontrado == false)
                    {
                        Console.WriteLine("\nEl numero de cuenta no existe!!! Pulse para continuar...");
                        Console.ReadKey();
                    }
                    else if (estado == "Bloqueada")
                    {
                        Console.WriteLine("\nNo se puede emitir una liquidacion a una tarjeta bloqueada. Pulse para continuar...");
                        Console.ReadKey();
                    }
                    else
                    {
                        Console.WriteLine("Periodo (YYYY-MM): ");
                        string periodo = Console.ReadLine()?? "";
                        Console.WriteLine("Fecha de Vencimiento (YYYY-MM-DD): ");
                        string fecha_input = Console.ReadLine()?? "";
                        DateTime fecha_vencimiento = Convert.ToDateTime(fecha_input);
                        Console.WriteLine("Total a pagar: ");
                        decimal total_a_pagar = Convert.ToDecimal(Console.ReadLine());
                        Console.WriteLine("Pago minimo: ");
                        decimal pago_minimo = Convert.ToDecimal(Console.ReadLine());

                       query = "INSERT INTO liquidaciones (num_cuenta, periodo, fecha_vencimiento, total_a_pagar, pago_minimo) VALUES (@num_cuenta, @periodo, @fecha_vencimiento, @total_a_pagar, @pago_minimo)";

                        using(MySqlCommand comando1 = new MySqlCommand(query, conexion))
                        {
                            comando1.Parameters.AddWithValue("@num_cuenta",numCuenta);
                            comando1.Parameters.AddWithValue("@periodo",periodo);
                            comando1.Parameters.AddWithValue("@fecha_vencimiento",fecha_vencimiento);
                            comando1.Parameters.AddWithValue("@total_a_pagar",total_a_pagar);
                            comando1.Parameters.AddWithValue("@pago_minimo",pago_minimo);

                            int filasLiquidacion = comando1.ExecuteNonQuery();

                            if (filasLiquidacion > 0)
                            {
                                Console.WriteLine("\nNueva liquidacion cargada al nro de cuenta "+numCuenta);
                                Console.WriteLine("\nPulse para continuar...");
                                Console.ReadKey();
                            }
                        }
                    }

                } catch(Exception ex)
                {
                    Console.WriteLine("Ocurrio un error...");
                }

            }
        }
        // =========================================================================
        // MÉTODOS BASE QUE DEBEN COMPLETAR CON LA LÓGICA 
        // =========================================================================

        static void altaClienteyTarjeta()
        {
            //Alta Cliente
            Console.Clear();
            Console.WriteLine("=== DATOS NUEVO CLIENTE ===");
            Console.WriteLine("Nro. Documento/Pasaporte (Sin puntos): ");
            string documento = Console.ReadLine()?? "";
            Console.WriteLine("Tipo documento (DNI o PASAPORTE): ");
            string tipo_doc = Console.ReadLine()?? "";
            Console.WriteLine("Nombre: ");
            string nombre = Console.ReadLine()?? "";
            Console.WriteLine("Apellido: ");
            string apellido = Console.ReadLine()?? "";
            Console.WriteLine("Fecha de Nacimiento (YYYY-MM-DD): ");
            string fecha_input = Console.ReadLine()?? "";
            DateTime fecha_nacimiento = Convert.ToDateTime(fecha_input);
            Console.WriteLine("Email: ");
            string email = Console.ReadLine()?? "";

            using(MySqlConnection conexion = new MySqlConnection(connectionString))
            {
                try
                {
                    conexion.Open();
                    string query = "INSERT INTO usuarios (documento, tipo_doc, nombre, apellido, fecha_nacimiento, email) VALUES (@documento, @tipo_doc, @nombre, @apellido, @fecha_nacimiento, @email)";

                    using (MySqlCommand comando = new MySqlCommand(query, conexion))
                    {
                        comando.Parameters.AddWithValue("@documento",documento);
                        comando.Parameters.AddWithValue("@tipo_doc",tipo_doc);
                        comando.Parameters.AddWithValue("@nombre",nombre);
                        comando.Parameters.AddWithValue("@apellido",apellido);
                        comando.Parameters.AddWithValue("@fecha_nacimiento",fecha_nacimiento);
                        comando.Parameters.AddWithValue("@email",email);

                        int filasCargadas = comando.ExecuteNonQuery();

                        if(filasCargadas > 0)
                        {
                            Console.WriteLine("\nUsuario creado con exito");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ocurrio un error al dar de alta al cliente");
                    Console.WriteLine(ex.Message);
                    Console.ReadKey();
                }
            }

            // Alta Tarjeta con documento del Cliente
            Console.WriteLine("=== DATOS NUEVA TARJETA (cliente "+documento+") ===");
            Console.WriteLine("Nro. Tarjeta (Sin espacios, 16 digitos): ");
            string numero_tarjeta = Console.ReadLine()?? "";
            Console.WriteLine("Indique nro banco emisor (1-Nación, 2-Provincia, 3-Galicia, 4-Santander, 5-BBVA, 6-Macro): ");
            string opcion = Console.ReadLine()?? "";
            string banco_emisor="";
            bool valido = false;
            while(!valido)
            {
                switch (opcion)
                {
                    case "1": banco_emisor = "Banco Nación"; valido = true; break;
                    case "2": banco_emisor = "Banco Provincia"; valido = true; break;
                    case "3": banco_emisor = "Banco Galicia"; valido = true; break;
                    case "4": banco_emisor = "Banco Santander"; valido = true; break;
                    case "5": banco_emisor = "Banco BBVA"; valido = true; break;
                    case "6": banco_emisor = "Banco Macro"; valido = true; break;
                    default:
                        Console.WriteLine("Opcion invalida, ingrese un nro del 1-6");
                        opcion = Console.ReadLine()?? "";
                        break;
                }
            }
            Console.WriteLine("Estado (Activa o Bloqueada): ");
            string estado = Console.ReadLine()?? "";
            Console.WriteLine("Saldo: ");
            string saldo = Console.ReadLine()?? "";
            Console.WriteLine("Titular: " + documento);
            string dni_titular = documento;
            Console.WriteLine("Se cargara la tarjeta al cliente "+ documento);

            using(MySqlConnection conexion = new MySqlConnection(connectionString))
            {
                try
                {   
                    conexion.Open();
                    string query = "INSERT INTO tarjetas (numero_tarjeta, banco_emisor, estado, saldo, dni_titular) VALUES (@numero_tarjeta, @banco_emisor, @estado, @saldo, @dni_titular)";

                    using(MySqlCommand comando = new MySqlCommand(query,conexion))
                    {
                        comando.Parameters.AddWithValue("@numero_tarjeta",numero_tarjeta);
                        comando.Parameters.AddWithValue("@banco_emisor",banco_emisor);
                        comando.Parameters.AddWithValue("@estado",estado);
                        comando.Parameters.AddWithValue("@saldo",saldo);
                        comando.Parameters.AddWithValue("@dni_titular",dni_titular);

                        int filasCargadas = comando.ExecuteNonQuery();

                        if(filasCargadas > 0)
                        {
                            Console.WriteLine("\nTarjeta creada con exito, pulse para continuar...");
                            Console.ReadKey();
                        }
                    }

                }
                catch (Exception ex)
                {   
                    Console.WriteLine("Ocurrio un error al dar de alta la tarjeta");
                    Console.WriteLine(ex.Message);
                    Console.ReadKey();
                }
            }

        }
        static void ObtenerYMostrarTarjetas()
        {
            // Completar 
            // Ejemplo de impresión dentro del bucle: 
            // Console.WriteLine("{0,-12} {1,-18} {2,-20} {3,-15}", reader["num_cuenta"], reader["numero_tarjeta"], ...);

            using (MySqlConnection conexion = new MySqlConnection(connectionString))
            {
                try
                {
                    conexion.Open();
                    string query = "SELECT num_cuenta, numero_tarjeta, banco_emisor, dni_titular FROM tarjetas";

                    using (MySqlCommand comando = new MySqlCommand(query, conexion))
                    {
                        using(MySqlDataReader lector = comando.ExecuteReader())
                        {
                            while(lector.Read())
                            {
                               // string num_cuenta = lector["num_cuenta"].ToString()??"";
                               // string numero_tarjeta = lector["numero_tarjeta"].ToString()??"";
                               // string banco_emisor = lector["banco_emisor"].ToString()??"";
                               // string dni_titular = lector["dni_titular"].ToString()??"";

                                Console.WriteLine("{0,-12} {1,-18} {2,-20} {3,-15}", lector["num_cuenta"], lector["numero_tarjeta"], lector["banco_emisor"], lector["dni_titular"]);
                                //funciona sin el signo $ y las {} ¿?¿? 
                            }
                        }

                    }
                }
                catch
                {

                }
            }
        }

        static void MostrarDetalleCompleto(int cuenta)
        {
            // Completar haciendo un SELECT con JOIN de usuarios y tarjetas WHERE num_cuenta = @cuenta

            using (MySqlConnection conexion = new MySqlConnection(connectionString))
            {
                try
                {
                    conexion.Open();
                    string query = "SELECT * FROM usuarios u JOIN tarjetas t ON u.documento = t.dni_titular WHERE t.num_cuenta = @cuenta";

                    using (MySqlCommand comando = new MySqlCommand(query,conexion))
                    {
                        comando.Parameters.AddWithValue("@cuenta",cuenta);

                        using (MySqlDataReader lector = comando.ExecuteReader())
                        {
                            if(lector.Read())
                            {
                                Console.WriteLine("Numero de cuenta: " + lector["num_cuenta"]);
                                Console.WriteLine("Apellido: " + lector["apellido"]);
                                Console.WriteLine("Nombre: " + lector["nombre"]);
                                Console.WriteLine("DNI (cliente/titular tarjeta): " + lector["documento"]);
                                Console.WriteLine("Numero de tarjeta: " + lector["numero_tarjeta"]);
                                Console.WriteLine("Banco emiso: " + lector["banco_emisor"]);
                                Console.WriteLine("Estado: " + lector["estado"]);
                                Console.WriteLine("Saldo: " + lector["saldo"]);
                            }
                            else
                            {
                                Console.WriteLine("No se encontro ninguna tarjeta para el nro de cliente "+ cuenta);
                                Console.ReadKey();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ocurrio un error...");
                    Console.WriteLine(ex.Message);
                }
            }
        }

        static bool DarDeBajaTarjeta(int cuenta)
        {
            // Completar usando un DELETE FROM tarjetas WHERE num_cuenta = @cuenta
            using (MySqlConnection conexion = new MySqlConnection(connectionString))
            {
                try
                {
                    conexion.Open();
                    string query = "DELETE FROM tarjetas WHERE num_cuenta = @cuenta";

                    using(MySqlCommand comando = new MySqlCommand(query,conexion))
                    {
                        comando.Parameters.AddWithValue("@cuenta", cuenta);

                        int clienteEliminado = comando.ExecuteNonQuery();

                        if(clienteEliminado > 0)
                        {
                            return true;
                        }

                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Ocurrio un error...");
                }
            }
            return false;
        }
    }
}