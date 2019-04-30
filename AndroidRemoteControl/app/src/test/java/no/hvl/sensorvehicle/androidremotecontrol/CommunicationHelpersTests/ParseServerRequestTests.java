package no.hvl.sensorvehicle.androidremotecontrol.CommunicationHelpersTests;

import org.junit.Test;

import java.util.HashMap;

import no.hvl.sensorvehicle.androidremotecontrol.CommunicationHelpers.Constants.ComponentType;
import no.hvl.sensorvehicle.androidremotecontrol.CommunicationHelpers.Constants.Key;
import no.hvl.sensorvehicle.androidremotecontrol.CommunicationHelpers.GenerateServerRequest;
import no.hvl.sensorvehicle.androidremotecontrol.CommunicationHelpers.ParseServerRequest;

import static org.junit.Assert.*;

/**
 * @see <a href="http://d.android.com/tools/testing">Testing documentation</a>
 */
public class ParseServerRequestTests {
    /*@Test
    public void toKeyValuePair_isCorrect() {
        HashMap<String, String> expected = new HashMap<>();
        expected.put(ComponentType.Ultrasound, "Left: 11,  Fwd: 22,  Right: 33.");

        assertEquals(expected,
                ParseServerRequest.toKeyValuePair("{ \"Ultrasound\": \"Left: 11,  Fwd: 22,  Right: 33.\" }"));
    }*/
}
