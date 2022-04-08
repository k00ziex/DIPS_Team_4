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
                .serverHost("626582a1d37a4c9da269c096cf520060.s1.eu.hivemq.cloud")
                .serverPort(8883)
                .sslWithDefaultConfig()
                .useMqttVersion5().buildAsync();

        mqttClient.connectWith()
                .simpleAuth()
                .username("dipsgrp4")
                .password("Dipsgrp4password".getBytes())
                .applySimpleAuth()
                .send()
                .whenComplete((connAck, thrower) -> {
                    if(thrower != null) {
                        // handle failure
                        Log.e(TAG, "Error in authentication to the server");
                    } else {
                        // setup subscribes or start publishing
                        mqttClient.subscribeWith()
                                .topicFilter("android/warning")
                                .callback(publish -> {
                                    // Process the received message
                                    String mes = new String(publish.getPayloadAsBytes(), StandardCharsets.UTF_8);
                                    Log.d(TAG, "We received a message");
                                })
                                .send()
                                .whenComplete((subAck, throwable) -> {
                                    if (throwable != null) {
                                        Log.e(TAG, "Error in Subscribing to Topic");
                                    } else {
                                        Log.i(TAG, "Successfully subcribed to topic");
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