import unittest
import random
from ImprovedBullyProcess import ImprovedBullyProcess
from OriginalBullyProcess import OriginalBullyProcess

class ComparisonCases(unittest.TestCase):
    def test_Original_10AliveNodes(self):
        listOfP = []
        networkMessagesSent = [0]

        for i in range(1,10):
            process = OriginalBullyProcess(i, networkMessagesSent)
            listOfP.append(process)
                
        for p in listOfP:
            p.setOtherProcessIDs(listOfP)

        listOfP[0].startElection()

        for p in listOfP:
            print(f"Process with id: {p.id} says leader is: {p.whoseLeader}")
        
        print(f"Messages Sent in Network: {networkMessagesSent[0]}")

    def test_improved_10_nodes_none_dead(self): 
        listOfP = []
        networkMessagesSent = [0]

        for i in range(1,10):
            process = ImprovedBullyProcess(i, networkMessagesSent)
            listOfP.append(process)

        for p in listOfP:
            p.setNeighbors(listOfP)

        listOfP[0].startElection()

        for p in listOfP:
            print(f"Process with id: {p.id} says leader is: {p.whoseLeader}")
        
        print(f"Messages Sent in Network: {networkMessagesSent[0]}")


    def test_Original_100AliveNodes_NonShuffledListOfProcesses(self):
        listOfP = []
        networkMessagesSent = [0]
        n_nodes = 100
        for i in range(0,n_nodes):
            process = OriginalBullyProcess(i, networkMessagesSent)
            listOfP.append(process)
        
        for p in listOfP:
            p.setOtherProcessIDs(listOfP)

        listOfP[0].startElection()

        for p in listOfP:
            print(f"Process with id: {p.id} says leader is: {p.whoseLeader}")
            self.assertEqual(p.whoseLeader, len(listOfP) - 1)
        
        print(f"Messages Sent in Network: {networkMessagesSent[0]}")

    def test_Original_100AliveNodes_ShuffledListOfProcesses(self):
        listOfP = []
        networkMessagesSent = [0]
        n_nodes = 100

        for i in range(0, n_nodes):
            process = OriginalBullyProcess(i, networkMessagesSent)
            listOfP.append(process)
        
        random.Random(42).shuffle(listOfP) # We seed Random with 42 to get the same randomness every time

        for p in listOfP:
            p.setOtherProcessIDs(listOfP)

        listOfP[0].startElection()

        for p in listOfP:
            print(f"Process with id: {p.id} says leader is: {p.whoseLeader}")
            self.assertEqual(p.whoseLeader, len(listOfP) - 1)
        
        print(f"Messages Sent in Network: {networkMessagesSent[0]}")

    def test_Improved_100AliveNodes_NonShuffledListOfProcesses(self):
        listOfP = []
        networkMessagesSent = [0]
        n_nodes = 100
        for i in range(0, n_nodes):
            process = ImprovedBullyProcess(i, networkMessagesSent)
            listOfP.append(process)
        
        for p in listOfP:
            p.setNeighbors(listOfP)

        listOfP[0].startElection()

        for p in listOfP:
            print(f"Process with id: {p.id} says leader is: {p.whoseLeader}")
            self.assertEqual(p.whoseLeader, len(listOfP) - 1)

        print(f"Messages Sent in Network: {networkMessagesSent[0]}")

    def test_Improved_100AliveNodes_ShuffledListOfProcesses(self):
        listOfP = []
        networkMessagesSent = [0]
        n_nodes = 100

        for i in range(0, n_nodes):
            process = ImprovedBullyProcess(i, networkMessagesSent)
            listOfP.append(process)
        
        random.Random(42).shuffle(listOfP) # We seed Random with 42 to get the same randomness every time

        for p in listOfP:
            p.setNeighbors(listOfP)

        listOfP[0].startElection()

        for p in listOfP:
            print(f"Process with id: {p.id} says leader is: {p.whoseLeader}")
            self.assertEqual(p.whoseLeader, len(listOfP) - 1)

        print(f"Messages Sent in Network: {networkMessagesSent[0]}")

    def test_Original_100AliveNodes_ShuffledListOfProcesses_EveryFifthNodeIsDead(self):
        listOfP = []
        networkMessagesSent = [0]
        n_nodes = 100

        for i in range(0, n_nodes):
            process = OriginalBullyProcess(i, networkMessagesSent)
            listOfP.append(process)
        
        random.Random(42).shuffle(listOfP) # We seed Random with 42 to get the same randomness every time

        ctr = 0
        for p in listOfP:
            p.setOtherProcessIDs(listOfP)
            ctr += 1
            if(ctr % 5 == 0): p.killNode()

        listOfP[0].startElection()

        for p in listOfP:
            print(f"Process with id: {p.id} says leader is: {p.whoseLeader}")
        
        print(f"Messages Sent in Network: {networkMessagesSent[0]}")

    def test_Improved_100AliveNodes_ShuffledListOfProcesses_EveryFifthProcessKilled(self):
        listOfP = []
        networkMessagesSent = [0]
        n_nodes = 100

        for i in range(0, n_nodes):
            process = ImprovedBullyProcess(i, networkMessagesSent)
            listOfP.append(process)
        
        random.Random(42).shuffle(listOfP) # We seed Random with 42 to get the same randomness every time
        ctr = 0
        for p in listOfP:
            p.setNeighbors(listOfP)
            ctr += 1
            if(ctr % 5 == 0): p.killNode()

        listOfP[0].startElection()

        for p in listOfP:
            print(f"Process with id: {p.id} says leader is: {p.whoseLeader}")

        print(f"Messages Sent in Network: {networkMessagesSent[0]}")

