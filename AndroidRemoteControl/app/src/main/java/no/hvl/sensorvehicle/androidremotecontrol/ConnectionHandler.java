package no.hvl.sensorvehicle.androidremotecontrol;

import android.os.AsyncTask;
import android.util.Log;

import java.io.IOException;
import java.io.PrintWriter;
import java.net.Socket;


public  class ConnectionHandler {

    // første melding som sendes virker, med neste er socked closed...??
    // legge til reader, kun etter hver gang det blir sendt en melding?
    // legge til broadcastR. for meldinger tilbake til activity? (tilkobling ok, mistettilkobling... )  ?





   final static String TAG = "ConnectionHandler";


    private static Socket socket;
    private static PrintWriter printWriter;
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



         if (socket.isClosed ()){
             Log.i (TAG, "reconnect");
             connect (connectedIP, connectedPort);

         }

         if (socket.isClosed ()){
             Log.i (TAG, " socket not connected");
             return "socket not connected";
         }
         SendMessageTask smTask = new SendMessageTask (message, socket);
        smTask.execute ();

        return status;
     }

    public static void closeSocket() throws IOException {
        if (socket.isClosed ())return;
        printWriter.close ();
        socket.close ();
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

             } catch (IOException e) {
                 e.printStackTrace ();
             }

             return null;
         }
     }

     static class SendMessageTask extends AsyncTask<Void, Void, Void>{
        String TAG = "SendMessageTask";
        String message;
        Socket s; // test
         public SendMessageTask(String message, Socket socket) {
             Log.i (TAG, "enter");
             this.message = message;
             this.s = socket;

         }

         @Override
         protected Void doInBackground(Void... voids) {
             try {
                 printWriter = new PrintWriter (s.getOutputStream ());
                 printWriter.write (message);
                 printWriter.flush ();
                 printWriter.close (); // sender ikke melding før den blir lukket??

             } catch (IOException e) {
                 e.printStackTrace ();
             }
             return null;
         }
     }
}
