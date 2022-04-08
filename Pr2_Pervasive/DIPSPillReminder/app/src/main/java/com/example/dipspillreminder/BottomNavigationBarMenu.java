package com.example.dipspillreminder;

import android.view.MenuItem;
import androidx.annotation.NonNull;
import androidx.fragment.app.Fragment;
import androidx.fragment.app.FragmentTransaction;
import com.google.android.material.navigation.NavigationBarView;

public class BottomNavigationBarMenu implements NavigationBarView.OnItemSelectedListener {

    private static BottomNavigationBarMenu instance;
    private MainActivity mainActivity;

    private BottomNavigationBarMenu(MainActivity mainActivity_) {
        mainActivity = mainActivity_;
    }

    public static BottomNavigationBarMenu getInstance(MainActivity mainActivity) {
        if(instance == null) {
            instance = new BottomNavigationBarMenu(mainActivity);
        }
        return instance;
    }

    @Override
    public boolean onNavigationItemSelected(@NonNull MenuItem item) {
        FragmentTransaction fragmentTransaction = mainActivity.getSupportFragmentManager().beginTransaction();
        Fragment fragment  = mainActivity.getSupportFragmentManager().findFragmentByTag(String.valueOf(R.string.PillReminderFragment));
        assert fragment != null;
        fragmentTransaction.replace(R.id.fragment_container, fragment, String.valueOf(R.string.PillReminderFragment));
        fragmentTransaction.commit();

        return false;
    }
}
