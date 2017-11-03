#pragma once

#include "node.hpp"
#include "skill.h"
#include "skill_factory.h"


namespace pvp {

//变量声明

//--------------------------------------------------------------------------------
// SkillCreator_22100
//
class SkillCreator_22100 : public SkillCreator {
public:
    SkillCreator_22100() {}
    virtual ~SkillCreator_22100() {}
    virtual SkillLogic* createSkill(Skill* skill, SkillableActor* owner, uint16_t nId);
    virtual bool getSkillExpend(int32_t& nType, int32_t& nValue);
}; // class SkillCreate_22100

//--------------------------------------------------------------------------------
// SkillPhase_22100
//
class SkillPhase_22100 : public SkillPhase {
public:
    SkillPhase_22100(uint16_t id, SkillLogic* logic, uint16_t nFlow) : SkillPhase(id, logic, nFlow) {}
    virtual ~SkillPhase_22100() {}

    virtual SkillPhaseNode* createPhaseNode(uint16_t nId, bool noInit) {
        SWITCH_CREATE_SKILL_START
        SWITCH_CREATE_SKILL_END
    }
}; // class SkillPhase_22100

SkillLogic* SkillCreator_22100::createSkill(Skill* skill, SkillableActor* owner, uint16_t nId) {
    //创建执行逻辑
    auto logic = new SkillLogic(nId, owner, skill);
    //创建阶段phase
    auto phase = new SkillPhase_22100(1, logic, 0);
    //设置phase执行器
    //添加设置phase
    logic->addPhase(phase);
    return logic;
}

bool SkillCreator_22100::getSkillExpend(int32_t& nType, int32_t& nValue) {
    return true;
}
} // namespace pvp
