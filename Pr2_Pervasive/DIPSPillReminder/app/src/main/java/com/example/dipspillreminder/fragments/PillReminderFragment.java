package com.example.dipspillreminder.fragments;

import static android.content.ContentValues.TAG;

import android.os.Bundle;

import androidx.fragment.app.Fragment;

import android.util.Log;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import com.example.dipspillreminder.R;
import com.hivemq.client.mqtt.MqttClient;
import com.hivemq.client.mqtt.mqtt5.Mqtt5AsyncClient;
import com.hivemq.client.mqtt.mqtt5.Mqtt5Client;

import java.io.Console;
import java.nio.charset.StandardCharsets;
import java.util.UUID;

public class PillReminderFragment extends Fragment {

    private static PillReminderFragment instance;
    private static Mqtt5AsyncClient mqttClient;


    private PillReminderFragment() {
    }

    public static PillReminderFragment getInstance() {
        if(instance == null) {
            instance = new PillReminderFragment();
        }
        return instance;
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);

        mqttClient = MqttClient.builder()
                .identifier(UUID.randomUUID().toString())
                .serverHost(getString(R.string.HiveMQ_Mqtt_Host))
                .serverPort(Integer.parseInt(getString(R.string.HiveMQ_Mqtt_Port)))
                .useMqttVersion5().buildAsync();

        mqttClient.connectWith()
                .simpleAuth()
                .username(String.valueOf(R.string.HiveMQ_Mqtt_Username))
                .password(String.valueOf(R.string.HiveMQ_Mqtt_Password).getBytes())
                .applySimpleAuth()
                .send()
                .whenComplete((connAck, thrower) -> {
                    if(thrower != null) {
                        // handle failure
                        Log.d(TAG, "onCreate: YIKES");
                    } else {
                        // setup subscribes or start publishing
                        mqttClient.subscribeWith()
                                .topicFilter("android/warning")
                                .callback(publish -> {
                                    // Process the received message
                                })
                                .send()
                                .whenComplete((subAck, throwable) -> {
                                    if (throwable != null) {
                                        Log.d(TAG, "onCreate: YIIIIKES2");
                                    } else {
                                        Log.i(TAG, "onCreate: WE RECEIECED SHIT");
                                    }
                                });
                    }
                });


    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment
        return inflater.inflate(R.layout.fragment_pill_reminder, container, false);
    }
}