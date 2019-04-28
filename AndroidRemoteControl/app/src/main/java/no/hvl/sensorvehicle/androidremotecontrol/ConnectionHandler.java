package no.hvl.sensorvehicle.androidremotecontrol;

import android.os.AsyncTask;
import android.util.Log;

import java.io.BufferedReader;
import java.io.BufferedWriter;
import java.io.IOException;
import java.io.InputStreamReader;
import java.io.OutputStreamWriter;
import java.net.Socket;

import no.hvl.sensorvehicle.androidremotecontrol.CommunicationHelpers.GenerateServerRequest;


public  class ConnectionHandler {
    // legge til broadcastR. for meldinger tilbake til activity? (tilkobling ok, mistettilkobling... )  ?

    final static String TAG = "ConnectionHandler";

    private static Socket socket;
    private static BufferedWriter bufferedWriter;  // Use BufferedWriter instead of PrintWriter or PrintStream over network:  https://stackoverflow.com/questions/13057740/printwriter-does-not-send-my-string-tcp-ip
    private static BufferedReader bufferedReader;
    private static String connectedIP;
    private static int connectedPort;

    public static String connect(String ip, int port){
        Log.i (TAG, "connect " + ip+ "  "+ port) ;
         String status ="Connecting "+ ip;
        connectedIP = ip;
        connectedPort = port;

        ConnectTask cTask = new ConnectTask ( ip, port);
        cTask.execute ();

         return status;
     }

     public static String sendMessage (String message){
        Log.i (TAG, "sendMessage :-> " + message);
         String status = "";

         /* TODO: connect() uses ConnectionTask, which runs asynchronously, and therefore new next if-statement may be entered before connection is complete.
         Consider removing ConnectTask, and let SendMessageTask handle connection if none is open.
         (Connection logic may be placed in a private method).*/
         if (socket.isClosed ()){
             Log.i (TAG, "reconnect");
             connect (connectedIP, connectedPort);
         }
         if (socket.isClosed ()){
             Log.i (TAG, " socket not connected");
             return "socket not connected";
         }

         SendMessageTask smTask = new SendMessageTask (message, bufferedWriter, bufferedReader);
         smTask.execute ();

        return status;
     }

    // TODO: Consider making private, and handle closing internally
    // (sendMessage(GenerateServerRequest.exitMessage()) should be used for closing connection - upon response from the server.
    // But we need to be able to close the connection also if the server doesn't respond for some reason.
    public static void closeSocket() throws IOException {
        if (socket.isClosed ())return;
        bufferedWriter.close();
        bufferedReader.close();
        socket.close ();
        Log.i (TAG, "CLOSED socket (closeSocket() ran to completion)");
    }

    static class ConnectTask extends AsyncTask<Void, Void, Void> {
        String TAG = "ConnectTask";
        String ip;
        int port;

         public ConnectTask(String ip, int port) {
             Log.i (TAG, "");

             this.ip = ip;
             this.port = port;
         }

         @Override
         protected Void doInBackground(Void... voids) {
             Log.i (TAG, "doInBackGround");

             try {
                 socket = new Socket (ip, port);
                 bufferedWriter = new BufferedWriter(new OutputStreamWriter(socket.getOutputStream()));
                 bufferedReader = new BufferedReader(new InputStreamReader(socket.getInputStream()));
                 Log.i (TAG, "OPENED new socket and bufferedWriter/Reader (ConnectTask ran to completion)");
             } catch (IOException e) {
                 e.printStackTrace ();
             }

             return null;
         }
     }

     static class SendMessageTask extends AsyncTask<Void, Void, Void>{
        String TAG = "SendMessageTask";
        String message;
        String response;
        BufferedWriter writer;
        BufferedReader reader;

         public SendMessageTask(String message, BufferedWriter writer, BufferedReader reader) {
             Log.i (TAG, "enter");
             this.message = message;
             this.writer = writer;
             this.reader = reader;
         }

         @Override
         protected Void doInBackground(Void... voids) {
             try {
                 if(!message.endsWith("\n")){
                     message = message + "\n";
                 }
                 bufferedWriter.write(message);
                 bufferedWriter.flush();
                 Log.i (TAG, "Message to server:\n " + message);

                 response = bufferedReader.readLine();
                 Log.i (TAG, "Response from server:\n " + response);
             } catch (IOException e) {
                 e.printStackTrace ();
             }
             return null;
         }

         @Override
         protected void onPostExecute(Void aVoid) {
             super.onPostExecute(aVoid);
             if(response.contentEquals("EXIT_CONFIRMATION")){
                 try {
                     closeSocket();
                 } catch (IOException e) {
                     e.printStackTrace();
                 }
             }
         }
     }
}
