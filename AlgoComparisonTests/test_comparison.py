import unittest
import random
from ImprovedBullyProcess import ImprovedBullyProcess
from OriginalBullyProcess import OriginalBullyProcess

class ComparisonCases(unittest.TestCase):
    def test_Original_10AliveNodes(self):
        listOfP = []
        networkMessagesSent = [0]

        for i in range(1,11):
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

    # 101 messages --> 1 Election + 1 ACK + 99 Coordinator msgs, ignore ACK --> 100 msgs
    def test_Improved_100AliveNodes_HighestIDAlive(self):
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

        print(f"Messages Sent in Network: {networkMessagesSent[0]}")
    
    # Adds 5 extra messages. 
    def test_Improved_100AliveNodes_FiveHighestIDsDead(self):
        listOfP = []
        networkMessagesSent = [0]
        n_nodes = 100

        for i in range(0, n_nodes):
            process = ImprovedBullyProcess(i, networkMessagesSent)
            listOfP.append(process)
        
        for p in listOfP:
            p.setNeighbors(listOfP)

        listOfP[len(listOfP) - 1].killNode()
        listOfP[len(listOfP) - 2].killNode()
        listOfP[len(listOfP) - 3].killNode()
        listOfP[len(listOfP) - 4].killNode()
        listOfP[len(listOfP) - 5].killNode()
        listOfP[0].startElection()
        

        for p in listOfP:
            print(f"Process with id: {p.id} says leader is: {p.whoseLeader}")

        print(f"Messages Sent in Network: {networkMessagesSent[0]}")


    # TODO: Does it make sense that this is only 106 messages?
    def test_Original_100AliveNodes_FiveHighestIDsDead(self):
        listOfP = []
        networkMessagesSent = [0]
        n_nodes = 100

        for i in range(0, n_nodes):
            process = OriginalBullyProcess(i, networkMessagesSent)
            listOfP.append(process)
        
    
        for p in listOfP:
            p.setOtherProcessIDs(listOfP)

        listOfP[len(listOfP) - 1].killNode()
        listOfP[len(listOfP) - 2].killNode()
        listOfP[len(listOfP) - 3].killNode()
        listOfP[len(listOfP) - 4].killNode()
        listOfP[len(listOfP) - 5].killNode()
        listOfP[0].startElection()

        for p in listOfP:
            print(f"Process with id: {p.id} says leader is: {p.whoseLeader}")
        
        print(f"Messages Sent in Network: {networkMessagesSent[0]}")

    #198 messages --> basically as bad as original + the latency will be big. 
    def test_Improved_100AliveNodes_StarterIsHighest(self):
        listOfP = []
        networkMessagesSent = [0]
        n_nodes = 100

        for i in range(0, n_nodes):
            process = ImprovedBullyProcess(i, networkMessagesSent)
            listOfP.append(process)
        
        for p in listOfP:
            p.setNeighbors(listOfP)

        # Kill nodes [99, 98... 1] only ID 0 is alive.
        for index in reversed(range(1, n_nodes)):
            listOfP[index].killNode()

        listOfP[0].startElection()

        for p in listOfP:
            print(f"Process with id: {p.id} says leader is: {p.whoseLeader}")

        print(f"Messages Sent in Network: {networkMessagesSent[0]}")

