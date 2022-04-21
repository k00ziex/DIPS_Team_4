package com.example.dipspillreminder.services;

import static android.content.ContentValues.TAG;

import android.app.Notification;
import android.app.NotificationChannel;
import android.app.NotificationManager;
import android.app.Service;
import android.content.Context;
import android.content.Intent;
import android.graphics.Color;
import android.media.RingtoneManager;
import android.net.Uri;
import android.os.IBinder;
import android.provider.Settings;
import android.util.Log;

import androidx.annotation.Nullable;
import androidx.core.app.NotificationCompat;
import androidx.core.app.NotificationManagerCompat;

import com.example.dipspillreminder.R;
import com.hivemq.client.mqtt.MqttClient;
import com.hivemq.client.mqtt.mqtt5.Mqtt5AsyncClient;

import java.nio.charset.StandardCharsets;
import java.util.UUID;


public class NotificationServices extends Service {

    private static final int notificationId = 10002;
    private static Mqtt5AsyncClient mqttClient;
    final String CHANNELID = "Foreground NotificationService ID";
    final String CHANNELNAME = "Foreground NotificationService NAME";
    final private String MQTTReminderTopic ="dipsgrp4/outputs/smartphone/commands/pillreminder";

    @Override
    public int onStartCommand(Intent intent, int flags, int startId) {

        createNotificationChannel();

        startMqttListener();

        return super.onStartCommand(intent, flags, startId);
    }

    private void createNotificationChannel() {
        NotificationManager mNotificationManager = (NotificationManager) getSystemService(NOTIFICATION_SERVICE);

        NotificationChannel mChannel = new NotificationChannel(CHANNELID, CHANNELNAME, NotificationManager.IMPORTANCE_HIGH);
        Uri uri = Uri.parse("android.resource://"+getPackageName()+"/" + R.raw.defaultnotificationsound);

        mChannel.enableLights(true);
        mChannel.setSound(uri, null);
        mChannel.setLightColor(Color.RED);
        mChannel.enableVibration(true);
        mNotificationManager.createNotificationChannel(mChannel);

    }

    @Nullable
    @Override
    public IBinder onBind(Intent intent) {
        return null;
    }


    private void startMqttListener(){
        mqttClient = MqttClient.builder()
                .identifier(UUID.randomUUID().toString())
                .serverHost(getString(R.string.HiveMQ_Mqtt_Host))
                .serverPort(Integer.parseInt(getString(R.string.HiveMQ_Mqtt_Port)))
                .sslWithDefaultConfig()
                .useMqttVersion5().buildAsync();

        mqttClient.connectWith()
                .simpleAuth()
                .username(getString(R.string.HiveMQ_Mqtt_Username))
                .password(getString(R.string.HiveMQ_Mqtt_Password).getBytes())
                .applySimpleAuth()
                .send()
                .whenComplete((connAck, thrower) -> {
                    if(thrower != null) {
                        // handle failure
                        Log.e(TAG, "Error in authentication to the server");
                    } else {
                        // setup subscribes or start publishing
                        mqttClient.subscribeWith()
                                .topicFilter(MQTTReminderTopic)
                                .callback(publish -> {
                                    // Process the received message
                                    String mes = new String(publish.getPayloadAsBytes(), StandardCharsets.UTF_8);
                                    Log.d(TAG, String.format("Message Received %s", mes) );
                                    createMqttNotification();
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

    private void createMqttNotification(){
        NotificationCompat.Builder builder = new NotificationCompat.Builder(this, CHANNELID)
                .setSmallIcon(R.drawable.heart_svgrepo_com)
                .setContentTitle("Medicine Warning")
                .setAutoCancel(true)
                .setContentText("Good Morning, Remember to take your medicine")
                .setPriority(NotificationCompat.PRIORITY_MAX);
        NotificationManagerCompat notificationManager = NotificationManagerCompat.from(this);
        notificationManager.notify(notificationId, builder.build());

    }



}
