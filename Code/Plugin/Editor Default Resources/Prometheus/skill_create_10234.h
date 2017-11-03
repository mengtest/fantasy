#pragma once

#include "node.hpp"
#include "skill.h"
#include "skill_factory.h"


namespace pvp {

//变量声明

//--------------------------------------------------------------------------------
// SkillCreator_10234
//
class SkillCreator_10234 : public SkillCreator {
public:
    SkillCreator_10234() {}
    virtual ~SkillCreator_10234() {}
    virtual SkillLogic* createSkill(Skill* skill, SkillableActor* owner, uint16_t nId);
    virtual bool getSkillExpend(int32_t& nType, int32_t& nValue);
}; // class SkillCreate_10234

//--------------------------------------------------------------------------------
// SkillPhase_10234
//
class SkillPhase_10234 : public SkillPhase {
public:
    SkillPhase_10234(uint16_t id, SkillLogic* logic, uint16_t nFlow) : SkillPhase(id, logic, nFlow) {}
    virtual ~SkillPhase_10234() {}

    virtual SkillPhaseNode* createPhaseNode(uint16_t nId, bool noInit) {
        SWITCH_CREATE_SKILL_START
        SWITCH_CREATE_SKILL_END
    }
}; // class SkillPhase_10234

SkillLogic* SkillCreator_10234::createSkill(Skill* skill, SkillableActor* owner, uint16_t nId) {
    //创建执行逻辑
    auto logic = new SkillLogic(nId, owner, skill);
    //创建阶段phase
    auto phase = new SkillPhase_10234(1, logic, 0);
    //设置phase执行器
    //添加设置phase
    logic->addPhase(phase);
    return logic;
}

bool SkillCreator_10234::getSkillExpend(int32_t& nType, int32_t& nValue) {
    return true;
}
} // namespace pvp
