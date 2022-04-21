package com.example.dipspillreminder.fragments;

import android.app.ActivityManager;
import android.content.Context;
import android.content.Intent;
import android.os.Bundle;

import androidx.fragment.app.Fragment;

import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;

import com.example.dipspillreminder.R;
import com.example.dipspillreminder.services.NotificationServices;

public class PillReminderFragment extends Fragment {


    public PillReminderFragment() {
    }

    @Override
    public void onCreate(Bundle savedInstanceState) {



        if(!foregroundServiceRunning())
        {
            Intent serviceIntent = new Intent(requireContext(), NotificationServices.class);
            requireContext().startService(serviceIntent);
        }

        super.onCreate(savedInstanceState);
    }

    @Override
    public View onCreateView(LayoutInflater inflater, ViewGroup container,
                             Bundle savedInstanceState) {
        // Inflate the layout for this fragment
        return inflater.inflate(R.layout.fragment_pill_reminder, container, false);
    }

    @SuppressWarnings("deprecation")
    public boolean foregroundServiceRunning(){
        ActivityManager activityManager = (ActivityManager) requireContext().getSystemService(Context.ACTIVITY_SERVICE);
        for(ActivityManager.RunningServiceInfo service: activityManager.getRunningServices(Integer.MAX_VALUE)) {
            if(NotificationServices.class.getName().equals(service.service.getClassName())) {
                return true;
            }
        }
        return false;
    }



}