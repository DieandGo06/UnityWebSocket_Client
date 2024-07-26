const express = require('express')
const app = express()
const server = require('http').createServer(app);
const WebSocket = require('ws');

const wss = new WebSocket.Server({ server: server });


//El servidor se conecta recbe datos de los clientes
wss.on('connection', function connection(ws) {

  //======================================================= Cuando se conecta un cliente =======================================================
  //Aviso en la consola/terminal
  console.log('Servidor: Se conecto un nuevo cliente');
  //El servidor envia un mensaje a los clientes para comprobar la conexión
  ws.send("Servidor: Clientes, se coencto otro de ustedes");
  //============================================================================================================================================


  //=================================================== Cuando recibe mensajes de un cliente ===================================================
  ws.on('message', function incoming(message) {
    console.log('received: ', message);

    //Envia mensajes a los clientes
    wss.clients.forEach(function each(client) {
      //if (client !== ws && client.readyState === WebSocket.OPEN) { //Si el cliente no es el mismo que envia la data y el servidor anda...
      client.send(message);
      //}
    });
  });
  //============================================================================================================================================
});

app.get('/', (req, res) => res.send('Hello World!'))
server.listen(3000, () => console.log(`Lisening on port :3000`))


//Si es la primera vez que abris el arhivo:
//1. Instala Node.JS que es el framework con el que se crea el serrvidor
//2. Abris la terminal de Visual y escribis el comando npm install para que se instalen las dependecnias necesarias


//Para levantar el servidor, abre el terninal y escribe: 
//npm start

//Posibles errores:
//Si al ejecutar "npm start" te dice que no existe ese comando
//Seguramente estas abriendo el archivo, y no la carpeta completa