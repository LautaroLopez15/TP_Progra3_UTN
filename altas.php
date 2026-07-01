<?php
$server = "localhost";
$user = "root";
$password = "root";
$baseDatos = "mi_banco_db";

$conexion = new mysqli($server,$user,$password,$baseDatos);

if($conexion->connect_error)
{
    die("Fallo al conectar: " . $conexion->connect_error);
}

$status = "";
$mensaje = "";

$doc_cliente = $_POST['documento'];
$sql = "SELECT documento FROM usuarios WHERE documento = $doc_cliente";

$resultado = $conexion->query($sql);
if($resultado->num_rows > 0)
{
    $usuario = $_POST['usuario'];
    $passwordA = $_POST['passwordA'];
    $passwordB = $_POST['passwordB'];

    if ($passwordA !== $passwordB)
    {
        $status = "error";
        $mensaje = "Las contraseñas ingresadas no coinciden.";
    }
    else
    {
        $sql = "UPDATE usuarios SET usuario = '$usuario' , password = '$passwordA' WHERE documento = '$doc_cliente'";

         if ($conexion->query($sql) === TRUE)
        {
            $status = "exito";
            $mensaje = "Se activo el usuario con exito, Bienvenido!!!";
        }
        else
        {
            $status = "error";
            $mensaje = "Error al validar usuario";
        }
    }    
}
else
{
    $status = "error";
    $mensaje = "El nro. de documento no pertenece a un cliente del banco";
}


$conexion->close();
?>

<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Mis Tarjetas - Estado de Activación</title>
    <script src="https://cdn.tailwindcss.com"></script>
</head>
<body class="bg-gray-100 font-sans min-h-screen flex flex-col justify-between">

    <header class="bg-[#004691] text-white text-center py-4 shadow-md">
        <h1 class="text-xl font-semibold">Mis <span class="font-bold">Tarjetas</span></h1>
    </header>

    <main class="flex-grow flex items-center justify-center p-6">
        <div class="bg-white rounded-lg shadow-lg max-w-md w-full p-8 text-center">
            
            <?php if ($status === "exito"): ?>
                <div class="w-16 h-16 bg-green-100 text-green-600 rounded-full flex items-center justify-center mx-auto mb-4">
                    <svg class="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7"></path></svg>
                </div>
                <h2 class="text-2xl font-bold text-green-600 mb-2">¡Activación Exitosa!</h2>
                <p class="text-gray-600 mb-6 text-sm"><?php echo $mensaje; ?></p>
                <a href="ingreso.html" class="inline-block w-full bg-[#004691] hover:bg-blue-800 text-white font-medium py-3 rounded-full transition duration-200">
                    Ir al Inicio de Sesión
                </a>

            <?php elseif ($status === "error"): ?>
                <div class="w-16 h-16 bg-red-100 text-red-500 rounded-full flex items-center justify-center mx-auto mb-4">
                    <svg class="w-8 h-8" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"></path></svg>
                </div>
                <h2 class="text-2xl font-bold text-red-600 mb-2">No se pudo activar</h2>
                <p class="text-gray-600 mb-6 text-sm"><?php echo $mensaje; ?></p>
                <a href="registro.html" class="inline-block w-full bg-gray-200 hover:bg-gray-300 text-gray-700 font-medium py-3 rounded-full transition duration-200">
                    Volver a Intentar
                </a>
            <?php endif; ?>
        </div>
    </main>

    <footer class="bg-gray-50 text-[10px] text-gray-500 text-center p-4 border-t border-gray-200">
        El Servicio Mis Tarjetas está disponible para todos aquellos socios que posean al menos una tarjeta Progra3card válida.
    </footer>
</body>
</html>