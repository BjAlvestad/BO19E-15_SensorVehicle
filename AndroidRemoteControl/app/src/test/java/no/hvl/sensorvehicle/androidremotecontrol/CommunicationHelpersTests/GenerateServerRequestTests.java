package no.hvl.sensorvehicle.androidremotecontrol.CommunicationHelpersTests;

import org.junit.Test;

import no.hvl.sensorvehicle.androidremotecontrol.CommunicationHelpers.GenerateServerRequest;

import static org.junit.Assert.*;

/**
 * @see <a href="http://d.android.com/tools/testing">Testing documentation</a>
 */
public class GenerateServerRequestTests {
    @Test
    public void setPowerString_isCorrect() {
        assertEquals("{ \"REQUEST_TYPE\": \"Command\", \"COMPONENT\": \"Wheel\", \"LEFT\": \"10\", \"RIGHT\": \"20\" }",
                GenerateServerRequest.setPower(10, 20));
    }

    @Test
    public void getSensorData_isCorrectFromOneSensor() {
        assertEquals("{ \"REQUEST_TYPE\": \"Data\", \"COMPONENT\": \"Ultrasound\" }",
                GenerateServerRequest.getSensorData("Ultrasound"));
    }

    @Test
    public void getSensorData_isCorrectFromTwoSensors() {
        assertEquals("{ \"REQUEST_TYPE\": \"Data\", \"COMPONENT\": \"Ultrasound Wheel\" }",
                GenerateServerRequest.getSensorData("Ultrasound", "Wheel"));
    }
}
