<?php
session_start();

if (!isset($_SESSION['logeado']) || $_SESSION['logeado'] !== true) {
    header("Location: ingreso.html");
    exit();
}

$server = "localhost";
$user = "root";
$password_db = "root";
$baseDatos = "mi_banco_db";

$conexion = new mysqli($server, $user, $password_db, $baseDatos);

if ($conexion->connect_error) {
    die("Fallo al conectar: " . $conexion->connect_error);
}


$documento = $_SESSION['documento'];
$sql = "SELECT l.num_cuenta, l.periodo, l.fecha_vencimiento, l.total_a_pagar, l.pago_minimo 
        FROM liquidaciones l
        JOIN tarjetas t ON l.num_cuenta = t.num_cuenta
        JOIN usuarios u ON t.dni_titular = u.documento
        WHERE u.documento = '$documento'
        ORDER BY l.periodo DESC";

$resultado = $conexion->query($sql);


$conexion->close();
?>

<!DOCTYPE html>
<html lang="es">
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Mis Tarjetas - Mis Liquidaciones</title>
    <script src="https://cdn.tailwindcss.com"></script>
</head>
<body class="bg-gray-100 font-sans min-h-screen flex flex-col justify-between">

    <header class="bg-[#004691] text-white py-4 px-6 shadow-md flex justify-between items-center">
        <h1 class="text-xl font-semibold">Mis <span class="font-bold">Tarjetas</span></h1>
        <div class="flex items-center space-x-4">
            <span class="text-sm hidden sm:inline">Hola, <strong><?php echo $_SESSION['apellido'] . " " . $_SESSION['nombre']; ?></strong></span>
        </div>
    </header>

    <main class="flex-grow p-4 md:p-8 max-w-5xl w-full mx-auto">
        <div class="bg-white rounded-lg shadow-lg p-6 md:p-8">
            
            <div class="border-b border-gray-200 pb-4 mb-6">
                <h2 class="text-2xl font-bold text-[#004691]">Resumen de Liquidaciones</h2>
                <p class="text-xs text-gray-500 mt-1">Documento: <?php echo $_SESSION['documento']; ?> | Usuario: <?php echo $_SESSION['nombre_usuario']; ?></p>
            </div>

            <div class="overflow-x-auto rounded-lg border border-gray-200">
                <table class="w-full text-sm text-left text-gray-500">
                    <thead class="text-xs uppercase bg-gray-50 text-gray-700 border-b border-gray-200">
                        <tr>
                            <th scope="col" class="px-6 py-3">Nro. Cuenta</th>
                            <th scope="col" class="px-6 py-3 text-center">Período</th>
                            <th scope="col" class="px-6 py-3 text-center">Vencimiento</th>
                            <th scope="col" class="px-6 py-3 text-right">Pago Mínimo</th>
                            <th scope="col" class="px-6 py-3 text-right">Total a Pagar</th>
                        </tr>
                    </thead>
                    <tbody class="divide-y divide-gray-200 bg-white">
                        
                        <?php if ($resultado->num_rows > 0): ?>

                            <?php while ($liquidacion = $resultado->fetch_assoc()): ?>
                                <tr class="hover:bg-gray-50 transition">
                                    <td class="px-6 py-4 font-medium text-gray-900">
                                        <?php echo $liquidacion['num_cuenta']; ?>
                                    </td>
                                    <td class="px-6 py-4 text-center whitespace-nowrap">
                                        <span class="bg-blue-50 text-blue-700 text-xs font-semibold px-2.5 py-0.5 rounded border border-blue-100">
                                            <?php echo date("m-Y", strtotime($liquidacion['periodo'] . "-01")); ?>
                                        </span>
                                    </td>
                                    <td class="px-6 py-4 text-center text-gray-600">
                                        <?php echo date("d/m/Y", strtotime($liquidacion['fecha_vencimiento'])); ?>
                                    </td>
                                    <td class="px-6 py-4 text-right font-medium text-gray-600">
                                        $<?php echo number_format($liquidacion['pago_minimo'], 2, ',', '.'); ?>
                                    </td>
                                    <td class="px-6 py-4 text-right font-bold text-[#004691] text-base">
                                        $<?php echo number_format($liquidacion['total_a_pagar'], 2, ',', '.'); ?>
                                    </td>
                                </tr>
                            <?php endwhile; ?>
                        
                        <?php else: ?>
                            <tr>
                                <td colspan="5" class="px-6 py-12 text-center text-gray-400">
                                    <svg class="w-12 h-12 mx-auto mb-3 text-gray-300" fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"></path></svg>
                                    <p class="text-sm font-medium">No se encontraron liquidaciones registradas para esta cuenta.</p>
                                </td>
                            </tr>
                        <?php endif; ?>

                    </tbody>
                </table>
            </div>

        </div>
    </main>

    <footer class="bg-gray-50 text-[10px] text-gray-500 text-center p-4 border-t border-gray-200">
        El Servicio Mis Tarjetas está disponible para todos aquellos socios que posean al menos una tarjeta Progra3card válida.
    </footer>

</body>
</html>