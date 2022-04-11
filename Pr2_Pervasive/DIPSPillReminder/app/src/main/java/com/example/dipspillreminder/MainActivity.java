package com.example.dipspillreminder;
import androidx.appcompat.app.AppCompatActivity;

import android.os.Bundle;

import com.example.dipspillreminder.fragments.PillReminderFragment;
import com.google.android.material.bottomnavigation.BottomNavigationView;

public class MainActivity extends AppCompatActivity {


    private PillReminderFragment pillReminderFragment;
    private BottomNavigationView bottomNavigationView;
    private BottomNavigationBarMenu bottomNavigationBarMenu;


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        pillReminderFragment = PillReminderFragment.getInstance();
        bottomNavigationBarMenu = BottomNavigationBarMenu.getInstance(this);

        bottomNavigationView = findViewById(R.id.bottom_navigation_bar);

        bottomNavigationView.setOnItemSelectedListener(bottomNavigationBarMenu);


        getSupportFragmentManager().beginTransaction()
                .replace(R.id.fragment_container, pillReminderFragment, String.valueOf(R.string.PillReminderFragment))
                .commit();
    }



}